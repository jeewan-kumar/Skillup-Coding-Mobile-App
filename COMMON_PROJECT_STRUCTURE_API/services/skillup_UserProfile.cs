using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace COMMON_PROJECT_STRUCTURE_API.services
{
    public class skillup_UserProfile
    {
        dbServices ds = new dbServices();
        public async Task<responseData> CreateProfile(requestData req)
        {
            responseData resData = new responseData();
            try
            {
                // Check if required parameters are present in the request
                if (!req.addInfo.ContainsKey("skillup_id") ||
                    !req.addInfo.ContainsKey("profile_picture") ||
                    !req.addInfo.ContainsKey("first_name") ||
                    !req.addInfo.ContainsKey("last_name") ||
                    !req.addInfo.ContainsKey("date_of_birth") ||
                    !req.addInfo.ContainsKey("gender") ||
                    !req.addInfo.ContainsKey("bio"))
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Missing required parameters";
                    return resData;
                }

                

                // Prepare SQL parameters for insertion
                MySqlParameter[] insertParams = new MySqlParameter[]
                {
                    new MySqlParameter("@skillup_id", req.addInfo["skillup_id"].ToString()),
                    new MySqlParameter("@profile_picture",req.addInfo["profile_picture"].ToString()),
                    new MySqlParameter("@first_name", req.addInfo["first_name"].ToString()),
                    new MySqlParameter("@last_name", req.addInfo["last_name"].ToString()),
                    new MySqlParameter("@date_of_birth", req.addInfo["date_of_birth"].ToString()),
                    new MySqlParameter("@gender", req.addInfo["gender"].ToString()),
                    new MySqlParameter("@bio", req.addInfo["bio"].ToString())
                };

                // Define the SQL query for insertion
                string query = @"
            INSERT INTO pc_student.Skillup_UserProfile
                (skillup_id, profile_picture, first_name, last_name, date_of_birth, gender, bio)
            VALUES
                (@skillup_id, @profile_picture, @first_name, @last_name, @date_of_birth, @gender, @bio)
        ";

                // Execute the SQL query
                var insertResult = ds.executeSQL(query, insertParams);

                // Check if insertion was successful
                if (insertResult == null || !insertResult.Any())
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Failed to create user profile";
                   
                }
                else
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "User profile created successfully";
                    // resData.rData["id"]=insertResult[0][0]["skillup_id"]int.TryParse;
                    
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "An error occurred: " + ex.Message;
            }
            return resData;
        }

        public async Task<responseData> ReadProfile(requestData req)
        {
            responseData resData = new responseData();
            try
            {
                MySqlParameter[] Params = new MySqlParameter[]
              {
                        new MySqlParameter("@id", req.addInfo["id"]),

              };
                var selectQuery = @"SELECT * FROM pc_student.Skillup_UserProfile where skillup_id=@skillup_id";

                var selectResult = ds.executeSQL(selectQuery, Params);
                if (selectResult[0].Count() == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "No UserProfile found";
                }
                else
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Userprofile retrieved Successfully";
                    resData.rData["lessons"] = selectResult;
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "An error occurred: " + ex.Message;
            }
            return resData;
        }

        public async Task<responseData> UpdateProfile(requestData req)
        {
            responseData resData = new responseData();
            try
            {
                byte[] imageData = null;

                // Handle profile picture if it is provided
                if (req.addInfo.ContainsKey("profile_picture") && !string.IsNullOrEmpty(req.addInfo["profile_picture"].ToString()))
                {
                    var filePath = req.addInfo["profile_picture"].ToString();
                    if (File.Exists(filePath))
                    {
                        imageData = File.ReadAllBytes(filePath);
                    }
                    else
                    {
                        resData.rData["rCode"] = 1;
                        resData.rData["rMessage"] = "File not found: " + filePath;
                        return resData;
                    }
                }

                // Prepare parameters for the SQL query
                List<MySqlParameter> updateParams = new List<MySqlParameter>
        {
            new MySqlParameter("@skillup_id", req.addInfo["skillup_id"].ToString()),
            new MySqlParameter("@first_name", req.addInfo["first_name"].ToString()),
            new MySqlParameter("@last_name", req.addInfo["last_name"].ToString()),
            new MySqlParameter("@date_of_birth", req.addInfo["date_of_birth"].ToString()),
            new MySqlParameter("@gender", req.addInfo["gender"].ToString()),
            new MySqlParameter("@bio", req.addInfo["bio"].ToString())
        };

                if (imageData != null)
                {
                    updateParams.Add(new MySqlParameter("@profile_picture", MySqlDbType.Blob) { Value = imageData });
                }
                else
                {
                    updateParams.Add(new MySqlParameter("@profile_picture", DBNull.Value));
                }

                // SQL query to update record
                var updateQuery = @"UPDATE pc_student.Skillup_UserProfile 
                            SET profile_picture = @profile_picture, 
                                first_name = @first_name, 
                                last_name = @last_name, 
                                date_of_birth = @date_of_birth, 
                                gender = @gender, 
                                bio = @bio  
                            WHERE skillup_id = @skillup_id";

                // Execute SQL update query
                var updateResult = ds.executeSQL(updateQuery, updateParams.ToArray());

                // Check if update was successful
                if (updateResult == null || !updateResult.Any())
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Unsuccessful profile update";
                }
                else
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Profile updated successfully";
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "An error occurred: " + ex.Message;
            }
            return resData;
        }


        public async Task<responseData> DeleteProfile(requestData req)
        {
            responseData resData = new responseData();
            try
            {
                // Create MySQL parameters for the delete query
                MySqlParameter[] deleteParams = new MySqlParameter[]
                {
            new MySqlParameter("@id", req.addInfo["id"].ToString()),
              new MySqlParameter("@status",0)
                };

                // Define the delete query
                var query = @"DELETE FROM pc_student.Skillup_UserProfile WHERE id = @id";
                //var query = @"UPDATE pc_student.Skillup_UserProfile SET status = @status WHERE id = @id";

                // Execute the delete query
                var deleteResult = ds.executeSQL(query, deleteParams);

                // Check the result of the delete operation
                if (deleteResult[0].Count() == 0 && deleteResult == null)
                {
                    resData.rData["rCode"] = 1; // Unsuccessful
                    resData.rData["rMessage"] = "Profile Unsuccessful delete";
                }
                else
                {
                    resData.rData["rCode"] = 0; // Successful
                    resData.rData["rMessage"] = "profile delete Successful";
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the operation
                resData.rData["rCode"] = 1; // Indicate an error
                resData.rData["rMessage"] = "Error: " + ex.Message;
            }

            // Return the response data
            return resData;
        }

        public async Task<responseData> UpdateUserProfileImage(requestData req)
        {
            responseData resData = new responseData();
            try
            {
                byte[] imageData = null;

                // Check if the request contains a new image file to update
                if (req.addInfo.ContainsKey("profile_picture") && !string.IsNullOrEmpty(req.addInfo["profile_picture"].ToString()))
                {
                    var filePath = req.addInfo["profile_picture"].ToString();
                    if (File.Exists(filePath))
                    {
                        imageData = File.ReadAllBytes(filePath);
                    }
                    else
                    {
                        resData.rData["rCode"] = 1;
                        resData.rData["rMessage"] = "File not found: " + filePath;
                        return resData;
                    }
                }
                else
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "No profile picture provided";
                    return resData;
                }

                // Parameters for SQL query
                MySqlParameter[] updateParams = new MySqlParameter[]
                {
            new MySqlParameter("@skillup_id", req.addInfo["skillup_id"].ToString()),
            new MySqlParameter("@profile_picture", MySqlDbType.Blob) { Value = imageData }
                };

                // SQL query to update record
                var updateQuery = @"UPDATE pc_student.Skillup_UserProfile SET profile_picture = @profile_picture WHERE skillup_id = @skillup_id";

                // Execute SQL update query
                var updateResult = ds.executeSQL(updateQuery, updateParams);

                // Check if update was successful
                if (updateResult == null || !updateResult.Any())
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Unsuccessful profile picture update";
                }
                else
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "Profile picture updated successfully";
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "An error occurred: " + ex.Message;
            }
            return resData;
        }
        public async Task<responseData> GetUserProfile(requestData req)
        {
            responseData resData = new responseData();
            try
            {
                MySqlParameter[] Params = new MySqlParameter[]
                {
                    new MySqlParameter("@skillup_id", req.addInfo["skillup_id"]),
                };

                var selectQuery = @"
                    SELECT up.profile_picture, up.first_name, up.last_name, up.date_of_birth, up.bio, us.email, us.phone_number,
                           CONCAT(up.first_name, ' ', up.last_name) AS name, up.gender
                    FROM pc_student.Skillup_UserProfile up
                    JOIN pc_student.Skillup_UserSignUp us ON up.skillup_id = us.skillup_id
                    WHERE up.skillup_id = @skillup_id";

                var selectResult = ds.executeSQL(selectQuery, Params);
                if (selectResult == null || selectResult.Count == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "No UserProfile found";
                }
                else
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "User profile retrieved successfully";
                    resData.rData["profile"] = selectResult[0];
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "An error occurred: " + ex.Message;
            }
            return resData;
        }
        public async Task<responseData> UpdateUserProfile(requestData req)
        {
            responseData resData = new responseData();

            try
            {
                // Validate input parameters
                if (!req.addInfo.ContainsKey("skillup_id"))
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Skillup ID is required";
                    return resData;
                }

                // // Prepare profile picture data if provided
                // byte[] imageData = null;
                // if (req.addInfo.ContainsKey("profile_picture") && !string.IsNullOrEmpty(req.addInfo["profile_picture"].ToString()))
                // {
                //     var filePath = req.addInfo["profile_picture"].ToString();
                //     if (File.Exists(filePath))
                //     {
                //         imageData = File.ReadAllBytes(filePath);
                //     }
                //     else
                //     {
                //         resData.rData["rCode"] = 1;
                //         resData.rData["rMessage"] = "File not found: " + filePath;
                //         return resData;
                //     }
                // }

                // Prepare update parameters
                List<MySqlParameter> updateParams = new List<MySqlParameter>
        {
            new MySqlParameter("@skillup_id", req.addInfo["skillup_id"].ToString()),
            new MySqlParameter("@profile_picture", req.addInfo["profile_picture"].ToString()),
            new MySqlParameter("@first_name", req.addInfo["first_name"].ToString()),
            new MySqlParameter("@last_name", req.addInfo["last_name"].ToString()),
            new MySqlParameter("@date_of_birth", req.addInfo["date_of_birth"].ToString()),
            new MySqlParameter("@bio", req.addInfo["bio"].ToString()),
            new MySqlParameter("@email", req.addInfo["email"].ToString()),
            new MySqlParameter("@phone_number", req.addInfo["phone_number"].ToString()),
            new MySqlParameter("@gender", req.addInfo["gender"].ToString())
        };

                // // Add profile picture parameter if imageData is not null
                // if (imageData != null)
                // {
                //     updateParams.Add(new MySqlParameter("@profile_picture", MySqlDbType.Blob) { Value = imageData });
                // }
                // else
                // {
                //     updateParams.Add(new MySqlParameter("@profile_picture", DBNull.Value));
                // }

                // SQL query to update user profile data
                string updateQuery = @"
            UPDATE pc_student.Skillup_UserProfile up
            JOIN pc_student.Skillup_UserSignUp us ON up.skillup_id = us.skillup_id
            SET
                up.profile_picture = @profile_picture,
                up.first_name = @first_name,
                up.last_name = @last_name,
                up.date_of_birth = @date_of_birth,
                up.bio = @bio,
                us.email = @email,
                us.phone_number = @phone_number,
                up.gender = @gender
            WHERE
                up.skillup_id = @skillup_id;
        ";

                // Execute the update query
                var updateResult = ds.executeSQL(updateQuery, updateParams.ToArray());

                // Check if update was successful
                if (updateResult == null || updateResult.Count == 0)
                {
                    resData.rData["rCode"] = 1;
                    resData.rData["rMessage"] = "Failed to update user profile";
                }
                else
                {
                    resData.rData["rCode"] = 0;
                    resData.rData["rMessage"] = "User profile updated successfully";
                }
            }
            catch (Exception ex)
            {
                resData.rData["rCode"] = 1;
                resData.rData["rMessage"] = "An error occurred: " + ex.Message;
            }

            return resData;
        }


    }
}

