using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSVTrim
{
    public partial class Form1 : Form
    {
        String folderpath;
        String tail = ";;;;;;;;;;;;;;\n"; // A lot of empty (redundant) data fields in the output CSV file

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            folderpath = textBox1.Text;

            // If a valid foldername was entered in the TextBox, then...
            if (Directory.Exists(folderpath))
            {
                // A subfolder named "Trimmed" is created.
                string filedirectoryTrimmed = folderpath + "\\Trimmed";
                Directory.CreateDirectory(filedirectoryTrimmed);

                foreach (string filepath in Directory.EnumerateFiles(folderpath, "*.csv"))
                {
                    // For each file to be trimmed, a new file with its name + "Trimmed" is 
                    // created in the subfolder. The output is written to that file
                    string filenameTrimmed = Path.GetFileNameWithoutExtension(filepath);
                    string filePathTrimmed = filedirectoryTrimmed + "\\" + filenameTrimmed + "Trimmed.csv";

                    //Console.WriteLine("IN " + folderpath + ": " + filepath);
                    string output = "";
                    
                    using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader r = new StreamReader(fs))
                        {
                            string allRows = r.ReadToEnd();
                            string[] rows = allRows.Split('\n');

                            bool first = true;
                            foreach (string row in rows)
                            {
                                string rowTrim = row;
                                if (row.IndexOf('\r') > -1)
                                    rowTrim = row.Remove(row.IndexOf('\r'), 1);

                                if (first)  
                                {
                                    // First row is the header row - just copy it
                                    //output += rowTrim + "\n";

                                    // HACK! (HNV 2020.08.17): The header must look exactly like this (from an older CSV file...)
                                    output += "Activity;Activity Short description;Username;SSN;Fullname;MobilePhone;Activity start Date;" + 
                                      "Activity End Date;Initial password;Given name;Surname;Address;Secondary address;Postal code;City;" +
                                      "IDFromAdministrativeSystem;PrivateMobilePhone;Email;COAddress\n";

                                    first = false;
                                }
                                else
                                {
                                    // The only fields needed to create the DB's are: class name (repeated 2 times) and student number (field 3).
                                    // The name of the student (field 5) is nice to have in the file, but not used for the DB
                                    string[] items = rowTrim.Split(';');
                                    if (items.Length > 5)
                                    {
                                        int count = 0;
                                        while (count < 5)
                                        {
                                            output += items[count];
                                            if (count < 4)
                                            {
                                                output += ";";
                                            }
                                            else
                                            {
                                                output += tail;
                                            }
                                            count++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    using (FileStream fs = new FileStream(filePathTrimmed, FileMode.Append))
                    {
                        using (StreamWriter w = new StreamWriter(fs))
                        {
                            w.Write(output);
                        }
                    }
                }
            }
        }
    }
}
