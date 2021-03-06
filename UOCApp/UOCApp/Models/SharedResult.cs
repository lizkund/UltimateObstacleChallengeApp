﻿using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace UOCApp
{
    public class SharedResult
    {

        //		/* server-sided */
        //		CREATE TABLE result (
        //			result_id integer(50) AUTO_INCREMENT,
        //			date date,
        //			time decimal(50,4),
        //			ranked boolean,
        //			flagged boolean,
        //			student_name varchar(50),
        //			student_gender varchar(1),
        //			student_grade SMALLINT(10),
        //			school_name varchar(50),
        //			PRIMARY KEY (result_id)
        //		);

        //int result_id; assigned by database
        DateTime date { get; set; }
        Decimal time { get; set; }
        Boolean ranked { get; set; }
        Boolean flagged { get; set; }
        String student_name { get; set; }
        String student_gender { get; set; }
        int student_grade { get; set; }
        String school_name { get; set; }

        public SharedResult(DateTime date, Decimal time, Boolean ranked, Boolean flagged,
            String student_name, String student_gender, int student_grade, String school_name)
        {
            if (date != null && time != null && ranked != null && flagged != null &&
                student_name != null && student_gender != null && student_grade != null && school_name != null
            )
            {
				if (time < 30)// minimum acceptable time in seconds
				{ 
						throw new ArgumentException("Invalid Time");
				}

                if (App.swearHelper.IsSwear(student_name))
                {
                    throw new ArgumentException("Invalid Student Name.");
                }

                if (App.swearHelper.IsSwear(school_name))
                {
                    throw new ArgumentException("Invalid School Name.");
                }
					
                this.date = date;
                this.time = time;
                this.ranked = ranked;
                this.flagged = flagged;
                this.student_name = student_name;
                this.student_gender = student_gender;
                this.student_grade = student_grade;
                this.school_name = school_name;
            }
            else
            {
                throw new ArgumentException("Please enter all of your information.");
            }
        }

        // share (post) the result to the server
        public async Task<Boolean> share(Boolean Official)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "date", this.date.ToString("yyyy-MM-dd") },
                    { "time", this.time.ToString() },
                    { "student_name", this.student_name },
                    { "student_grade", this.student_grade.ToString() },
                    { "student_gender", this.student_gender },
                    { "school_name", this.school_name }

                };


                bool loggedIn = false;
                if (Application.Current.Properties.ContainsKey("loggedin"))
                {
                    loggedIn = Convert.ToBoolean(Application.Current.Properties["loggedin"]);
                }
                if (Official && loggedIn)
                {
                    values.Add("password", App.password); 
                }

                var content = new FormUrlEncodedContent(values);

				try {
	                var response = await client.PostAsync(App.API_URL + "result/", content);
					//Console.WriteLine("post response code: " + response.StatusCode);
					var responseString = await response.Content.ReadAsStringAsync(); //what does this do??
						if ((int)response.StatusCode == 201)
						{
							return true;
						}
						else
						{
							values.ToList().ForEach(x => Console.WriteLine(x.Key + " : " + x.Value));
							return false;
						}
				}
				catch(Exception e) { //pokemon
					return false;
				}


                

                
            }
        }
    }
}
