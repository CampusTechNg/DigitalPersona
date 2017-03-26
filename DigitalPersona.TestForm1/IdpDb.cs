using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalPersona.TestForm1
{
    public class IdpDb
    {
        string connectionString;
        MySqlConnection mySqlConnection;

        public IdpDb()
        {
            connectionString = "server=localhost;database=idp_test;uid=root;pwd=root;";
            mySqlConnection = new MySqlConnection(connectionString);
        }

        public bool Open()
        {
            try
            {
                mySqlConnection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Idp[] GetPersons()
        {
            MySqlCommand selectJoin = new MySqlCommand();
            selectJoin.Connection = mySqlConnection;
            selectJoin.CommandText =
                "SELECT b.id, first_name, last_name, other_names, dob, yob, gender, marital_status, state, lga, " +
                "f.finger_1, finger_2, finger_3, finger_4, finger_5, finger_6, finger_7, finger_8, finger_9, finger_10, " +
                "p.photo " +
                "from bios as b join photos as p on b.id = p.id join fingers as f on b.id = f.id";

            List<Idp> persons = new List<Idp>();
            try
            {
                mySqlConnection.Open();
                MySqlDataReader dr = selectJoin.ExecuteReader(System.Data.CommandBehavior.SequentialAccess);
                while (dr.Read())
                {
                    Idp person = new Idp();

                    person.FirstName = dr.GetString("first_name");
                    person.LastName = dr.GetString("last_name");
                    person.OtherNames = dr.GetString("other_names");
                    person.DoB = dr.GetDateTime("dob");
                    person.YoB = dr.GetInt32("yob");
                    person.Gender = dr.GetString("gender");
                    person.MaritalStatus = dr.GetString("marital_status");
                    person.State = dr.GetString("state");
                    person.LGA = dr.GetString("lga");

                    for (int index = 0; index < person.FingerTemplates.Length; index++)
                    {
                        int realIndex = index + 1;
                        int ordinal = dr.GetOrdinal("finger_" + realIndex);
                        if (!(dr.IsDBNull(ordinal)))
                        {
                            byte[] deserializedTemplate = (byte[])dr["finger_" + realIndex];
                            DPFP.Template template = new DPFP.Template();
                            template.DeSerialize(deserializedTemplate);
                            person.FingerTemplates[index] = template;
                        }
                    }

                    int phOrdinal = dr.GetOrdinal("photo");
                    if (!(dr.IsDBNull(phOrdinal)))
                    {
                        byte[] imgBytes = GetBytes(dr, phOrdinal);
                        person.Photo = ByteArrayToImage(imgBytes);
                    }

                    persons.Add(person);
                }
                dr.Close();
            }
            catch (Exception ex) { }
            finally
            {
                mySqlConnection.Close();
            }
            return persons.ToArray();
        }

        public bool SavePerson(Idp person, out string id)
        {
            MySqlCommand insertBio = new MySqlCommand(), 
                insertPhoto = new MySqlCommand(),
                insertFingers = new MySqlCommand();
            insertBio.Connection = insertPhoto.Connection = insertFingers.Connection = mySqlConnection;
            id = DateTime.Now.ToString();

            insertBio.CommandText =
                "INSERT INTO bios VALUES(@id, @first_name, @last_name, @other_names, @dob, @yob, @gender, @marital_status, @state, @lga)";
            insertBio.Parameters.Add("id", MySqlDbType.VarChar).Value = id;
            insertBio.Parameters.Add("first_name", MySqlDbType.VarChar).Value = person.FirstName;
            insertBio.Parameters.Add("last_name", MySqlDbType.VarChar).Value = person.LastName;
            insertBio.Parameters.Add("other_names", MySqlDbType.VarChar).Value = person.OtherNames;
            insertBio.Parameters.Add("dob", MySqlDbType.DateTime).Value = person.DoB;
            insertBio.Parameters.Add("yob", MySqlDbType.Int32).Value = person.YoB;
            insertBio.Parameters.Add("gender", MySqlDbType.VarChar).Value = person.Gender;
            insertBio.Parameters.Add("marital_status", MySqlDbType.VarChar).Value = person.MaritalStatus;
            insertBio.Parameters.Add("state", MySqlDbType.VarChar).Value = person.State;
            insertBio.Parameters.Add("lga", MySqlDbType.VarChar).Value = person.LGA;

            insertPhoto.CommandText =
                "INSERT INTO photos VALUES(@id, @photo)";
            insertPhoto.Parameters.Add("id", MySqlDbType.VarChar).Value = id;
            if (person.Photo != null)
            {
                var imagBytes = ImageToByteArray(person.Photo);
                //insertPhoto.Parameters.Add("photo_size", MySqlDbType.Int32).Value = imagBytes.Length;
                insertPhoto.Parameters.Add("photo", MySqlDbType.LongBlob).Value = imagBytes;
            }
            else
            {
                //insertPhoto.Parameters.Add("photo_size", MySqlDbType.Int32).Value = 0;
                insertPhoto.Parameters.Add("photo", MySqlDbType.LongBlob).Value = null;
            }

            insertFingers.CommandText =
                "INSERT INTO fingers VALUES(@id, @finger_1, @finger_2, @finger_3, @finger_4, @finger_5, @finger_6, @finger_7, @finger_8, @finger_9, @finger_10)";
            insertFingers.Parameters.Add("id", MySqlDbType.VarChar).Value = id;
            for(int index = 0; index < person.FingerTemplates.Length; index++)
            {
                var template = person.FingerTemplates[index];
                int realIndex = index + 1;
                if (template != null)
                {
                    byte[] serializedTemplate = null;
                    template.Serialize(ref serializedTemplate);
                    insertFingers.Parameters.Add("finger_" + realIndex, MySqlDbType.Blob).Value = serializedTemplate;

                    //string stringTemplate = System.Text.Encoding.UTF8.GetString(serializedTemplate);
                    //insertFingers.Parameters.Add("finger_" + (index + 1), MySqlDbType.LongText).Value = stringTemplate;
                }
                else
                {
                    insertFingers.Parameters.Add("finger_" + realIndex, MySqlDbType.Blob).Value = null;
                }
            }

            bool noErrorOccured = true;
            try
            {
                mySqlConnection.Open();
                insertBio.ExecuteNonQuery();
                insertPhoto.ExecuteNonQuery();
                insertFingers.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                noErrorOccured = false;
            }
            finally
            {
                mySqlConnection.Close();
            }
            return noErrorOccured;
        }

        byte[] ImageToByteArray(System.Drawing.Image image)
        {
            //using (var ms = new MemoryStream())
            //{
            //    image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif); //image.RawFormat
            //    var sss = ms.ToArray();
            //    return ms.ToArray();
            //}

            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(image, typeof(byte[]));
            return xByte;
        }
        System.Drawing.Image ByteArrayToImage(byte[] bytes)
        {
            //System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);

            ////return new System.Drawing.Bitmap(ms);
            //return System.Drawing.Image.FromStream(ms);

            //using (var ms = new MemoryStream(bytes))
            //{
            //    return System.Drawing.Image.FromStream(ms);
            //}

            System.Drawing.Image x = (System.Drawing.Bitmap)((new System.Drawing.ImageConverter()).ConvertFrom(bytes));
            return x;
        }
        byte[] GetBytes(MySqlDataReader reader, int ordinal)
        {
            //byte[] result = null;

            //long size = reader.GetBytes(ordinal, 0, null, 0, 0); //get the length of data 
            //result = new byte[size];
            //int bufferSize = 1024;
            //long bytesRead = 0;
            //int curPos = 0;
            //while (bytesRead < size)
            //{
            //    bytesRead += reader.GetBytes(ordinal, curPos, result, curPos, bufferSize);
            //    curPos += bufferSize;
            //}

            //return result;

            BinaryWriter bw;
            int bufferSize = 1024;                   // Size of the BLOB buffer.
            byte[] outbyte = new byte[bufferSize];  // The BLOB byte[] buffer to be filled by GetBytes.
            long retval;                            // The bytes returned from GetBytes.
            long startIndex = 0;                    // The starting position in the BLOB output.
            MemoryStream ms = new MemoryStream();
            bw = new BinaryWriter(ms);
            // Reset the starting byte for the new BLOB.
            startIndex = 0;

            // Read the bytes into outbyte[] and retain the number of bytes returned.
            retval = reader.GetBytes(ordinal, startIndex, outbyte, 0, bufferSize);

            // Continue reading and writing while there are bytes beyond the size of the buffer.
            while (retval == bufferSize)
            {
                bw.Write(outbyte);
                bw.Flush();

                // Reposition the start index to the end of the last buffer and fill the buffer.
                startIndex += bufferSize;
                retval = reader.GetBytes(ordinal, startIndex, outbyte, 0, bufferSize);
            }

            // Write the remaining buffer.
            bw.Write(outbyte, 0, (int)retval - 1);
            bw.Flush();

            byte[] b = ms.ToArray();

            // Close the output file.
            bw.Close();
            ms.Close();

            return b;
        }
    }
}
