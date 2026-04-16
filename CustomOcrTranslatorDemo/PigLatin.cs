/*
 * 	This demo shows how to write a custom translator.  This winform application uses
 * the custom translator from PigLatinTranslator.cs.  All the basics are shown here, 
 * but for more information please see the OCR Developer's Guide.
 * 
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using Atalasoft.Imaging;
using Atalasoft.Imaging.WinControls;
using Atalasoft.Ocr;
using Atalasoft.Imaging.Codec;
using Atalasoft.Ocr.Tesseract;
using WinDemoHelperMethods;

namespace CustomOcrTranslatorDemo
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button SelectImage;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton plainRadio;
		private System.Windows.Forms.RadioButton pigRadio;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private OcrEngine engine;
		private System.Windows.Forms.TextBox theText;
		private PigLatinTranslator anslatorTray;
		private System.Windows.Forms.Button AboutButton;
		private static string tempFile = Path.GetTempPath() + "tempOCR.txt";
		private bool _validLicense;

        static Form1()
        {
            HelperMethods.PopulateDecoders(RegisteredDecoders.Decoders);
        }

#region Windows Form Designer generated code
		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//Initialize the ocr engin only once, at start up.
			// setting the OcrResources path to null forces the OcrEngine to find them either from
			// the registry entry(hklm\software\atalasoft\dotimage\7.0) or in the same directory as the atalasoft dll's.
			try
			{
                engine = new Tesseract5Engine();
				_validLicense = true;
			}
			catch(AtalasoftLicenseException)
			{
				_validLicense = false;
				LicenseCheckFailure("This Demo cannot run without an Atalasoft OCR License, with Tesseract Engine.  Please request an evaluation license, or purchase one from www.Atalasoft.com");
				return;
			}
			engine.Initialize();
			// add the custom pigLatinTranslator to the collection of availible translators.
			// During translation, the OcrEngine will use the first translator, in the collection,
			// that supports the given mime type.
			anslatorTray = new PigLatinTranslator();
			engine.Translators.Add(anslatorTray);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SelectImage = new System.Windows.Forms.Button();
			this.plainRadio = new System.Windows.Forms.RadioButton();
			this.pigRadio = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.theText = new System.Windows.Forms.TextBox();
			this.AboutButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// SelectImage
			// 
			this.SelectImage.Location = new System.Drawing.Point(24, 16);
			this.SelectImage.Name = "SelectImage";
			this.SelectImage.Size = new System.Drawing.Size(128, 72);
			this.SelectImage.TabIndex = 0;
			this.SelectImage.Text = "Translate Image...";
			this.SelectImage.Click += new System.EventHandler(this.SelectImage_Click);
			// 
			// plainRadio
			// 
			this.plainRadio.Checked = true;
			this.plainRadio.Location = new System.Drawing.Point(216, 32);
			this.plainRadio.Name = "plainRadio";
			this.plainRadio.Size = new System.Drawing.Size(80, 24);
			this.plainRadio.TabIndex = 1;
			this.plainRadio.TabStop = true;
			this.plainRadio.Text = "Plain Text";
			this.plainRadio.CheckedChanged += new System.EventHandler(this.plainRadio_CheckedChanged);
			// 
			// pigRadio
			// 
			this.pigRadio.Location = new System.Drawing.Point(216, 56);
			this.pigRadio.Name = "pigRadio";
			this.pigRadio.Size = new System.Drawing.Size(80, 24);
			this.pigRadio.TabIndex = 2;
			this.pigRadio.Text = "Pig Latin";
			this.pigRadio.CheckedChanged += new System.EventHandler(this.pigRadio_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(200, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(112, 80);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Translate To";
			// 
			// theText
			// 
			this.theText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.theText.Location = new System.Drawing.Point(0, 96);
			this.theText.Multiline = true;
			this.theText.Name = "theText";
			this.theText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.theText.Size = new System.Drawing.Size(440, 440);
			this.theText.TabIndex = 4;
			this.theText.Text = "";
			// 
			// AboutButton
			// 
			this.AboutButton.Location = new System.Drawing.Point(352, 8);
			this.AboutButton.Name = "AboutButton";
			this.AboutButton.TabIndex = 5;
			this.AboutButton.Text = "About ...";
			this.AboutButton.Click += new System.EventHandler(this.AboutButton_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(440, 534);
			this.Controls.Add(this.AboutButton);
			this.Controls.Add(this.theText);
			this.Controls.Add(this.pigRadio);
			this.Controls.Add(this.plainRadio);
			this.Controls.Add(this.SelectImage);
			this.Controls.Add(this.groupBox1);
			this.Name = "Form1";
			this.Text = "Pig Latin Translator";
			this.ResumeLayout(false);

		}
		

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
#endregion
		
		#region License check code

		private void LicenseCheckFailure(string message)
		{
			this.Load += new System.EventHandler(this.Form1_Load);
			if (MessageBox.Show(this, message + "\r\n\r\nWould you like to request an evaluation license?", "License Required", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
			{
				// Locate the activation utility.
				string path = "";
				Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Atalasoft\dotImage\5.0");
				if (key != null)
				{
					path = Convert.ToString(key.GetValue("AssemblyBasePath"));
					if (path != null && path.Length > 5)
						path = path.Substring(0, path.Length - 3) + "AtalasoftToolkitActivation.exe";
					else
						path = Path.GetFullPath(@"..\..\..\..\..\AtalasoftToolkitActivation.exe");

					key.Close();
				}

				if (System.IO.File.Exists(path))
					System.Diagnostics.Process.Start(path);
				else
					MessageBox.Show(this, "We were unable to location the DotImage activation utility.\r\nPlease run it from the Start menu shortcut.", "File Not Found");
			}
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			// close the demo if there is no valid license
			if (!this._validLicense)
				Application.Exit();
		}

		#endregion

		// this method loads the image and translates it to the selected mime type.
		private void SelectImage_Click(object sender, System.EventArgs e)
		{
			OpenImageFileDialog oif = new OpenImageFileDialog();
            oif.Filter = HelperMethods.CreateDialogFilter(true);

			// try to locate images folder
			string imagesFolder = Application.ExecutablePath;
			// we assume we are running under the DotImage install folder
			int pos = imagesFolder.IndexOf("DotImage ");
			if (pos != -1)
			{
				imagesFolder = imagesFolder.Substring(0,imagesFolder.IndexOf(@"\",pos)) + @"\Images\OCR";
			}

			//use this folder as starting point			
			oif.InitialDirectory = imagesFolder;

			if (oif.ShowDialog(this) != DialogResult.OK) 
			{
				return;
			}

			string[] paths = new string[1];
			paths[0] = oif.FileName;
			oif.Dispose();
			// this is how the translate method takes images as input.
			FileSystemImageSource source = new FileSystemImageSource(paths, true);

			StreamReader reader = null;

			string mime = GetMimeType();
			// Translate the document.
			if (engine.CanStream(mime)) 
			{
				MemoryStream stream = new MemoryStream();
				try 
				{
					engine.Translate(source, mime, stream);
				}
				catch (Exception err)
				{
					MessageBox.Show("Caught an error: " + err.Message);
				}
				stream.Seek(0, SeekOrigin.Begin);
				reader = new StreamReader(stream);
			}
			else if (engine.CanTranslate(mime))  
			{
				// this mime type can only be output to a file directly
				try 
				{
					engine.Translate(source, mime, tempFile);
				}
				catch (Exception err) 
				{
					MessageBox.Show("Caught an error" + err.Message);
					if (File.Exists(tempFile)) 
					{
						File.Delete(tempFile);
					}
				}
				reader = new StreamReader(tempFile);
			}
			else 
			{
				MessageBox.Show(this, "Can't find a translator for type \"" + mime + "\"");
			}
			if (reader != null) // else the translate operation failed for some reason.
			{
				try {
					// read text back to text box.
					theText.Text = reader.ReadToEnd();
				}
				catch (Exception err) {
					MessageBox.Show("Caught an error reading in data: " + err.Message);
				}
				finally {
					reader.Close();
					if (File.Exists(tempFile))
						File.Delete(tempFile);
				}
			}
		}


		#region Handlers and Helpers

		// returns the currently selected mime type.
		private string GetMimeType()
		{
			return plainRadio.Checked ? "text/plain" : "text/pig-latin";
		}

		// event handler for radio buttons
		private void plainRadio_CheckedChanged(object sender, System.EventArgs e)
		{
			if (plainRadio.Checked) 
			{
				pigRadio.Checked = false;
			}
		}
		// event handler for radio buttons.
		private void pigRadio_CheckedChanged(object sender, System.EventArgs e)
		{
			if (pigRadio.Checked) 
			{
				plainRadio.Checked = false;
			}
		}
		#endregion

		private void AboutButton_Click(object sender, System.EventArgs e)
		{
			AtalaDemos.AboutBox.About aboutBox = new AtalaDemos.AboutBox.About("About Atalasoft DotImage Ocr Pig Latin Translator Demo",
				"DotImage Ocr Pig Latin Translator Demo");
			aboutBox.Description = @"Demonstrates how to create and use a custom OCR translator.  The translator implemented here will convert regular text to pig latin.  A custom translator will be useful if to convert documents to an unsupported (or custom) format.  Writing your own translator is relatively simple, and this demo serves as a good reference. ";
            aboutBox.ShowDialog();
		}
	}
}
