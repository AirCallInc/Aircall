using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Util;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace Aircall.admin.assets.ckeditor
{
    public partial class ImageBrowser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDirectoryList();
                ChangeDirectory(null, null);

                NewDirectoryButton.OnClientClick = "var name = prompt('Enter name of folder:'); if (name == null || name == '') return false; document.getElementById('" + NewDirectoryName.ClientID + "').value = name;";
            }

            ResizeMessage.Text = "";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "FocusImageList", "window.setTimeout(\"document.getElementById('" + ImageList.ClientID + "').focus();\", 1);", true);
        }

        protected void BindDirectoryList()
        {
            ImageFolder = "";
            string[] directories = Directory.GetDirectories(FileImageFolder);
            directories = Array.ConvertAll<string, string>(directories, delegate(string directory) { return directory.Substring(directory.LastIndexOf('\\') + 1); });
            DirectoryList.DataSource = directories;
            DirectoryList.DataBind();
            DirectoryList.Items.Insert(0, "Root");
        }

        protected void ChangeDirectory(object sender, EventArgs e)
        {
            DeleteDirectoryButton.Enabled = (DirectoryList.SelectedIndex != 0);
            ImageFolder = (DirectoryList.SelectedIndex == 0) ? "" : DirectoryList.SelectedValue + "/";
            SearchTerms.Text = "";
            BindImageList();
            SelectImage(null, null);
        }

        protected void DeleteFolder(object sender, EventArgs e)
        {
            Directory.Delete(FileImageFolder, true);
            BindDirectoryList();
            ChangeDirectory(null, null);
        }

        protected void CreateFolder(object sender, EventArgs e)
        {
            string name = UniqueDirectory(NewDirectoryName.Value);
            Directory.CreateDirectory(FileImageFolderRoot + name);
            BindDirectoryList();
            DirectoryList.SelectedValue = name;
            ChangeDirectory(null, null);
        }

        protected void BindImageList()
        {
            ImageList.Items.Clear();
            string[] files = Directory.GetFiles(FileImageFolder, "*" + SearchTerms.Text.Replace(" ", "*") + "*");
            files = Array.FindAll(files, delegate(string f) { return IsImage(f); });

            foreach (string file in files)
            {
                if (IsImage(file))
                    ImageList.Items.Add(file.Substring(file.LastIndexOf('\\') + 1));
            }

            if (files.Length > 0)
                ImageList.SelectedIndex = 0;
        }

        protected void Search(object sender, EventArgs e)
        {
            BindImageList();
        }

        protected void SelectImage(object sender, EventArgs e)
        {
            RenameImageButton.Enabled = (ImageList.Items.Count != 0);
            DeleteImageButton.Enabled = (ImageList.Items.Count != 0);
            ResizeImageButton.Enabled = (ImageList.Items.Count != 0);
            ResizeWidth.Enabled = (ImageList.Items.Count != 0);
            ResizeHeight.Enabled = (ImageList.Items.Count != 0);

            if (ImageList.Items.Count == 0)
            {
                Image1.ImageUrl = "";
                ResizeWidth.Text = "";
                ResizeHeight.Text = "";
                return;
            }

            Image1.ImageUrl = ImageFolder + ImageList.SelectedValue + "?" + new Random().Next(1000);
            ImageMedia img = ImageMedia.Create(File.ReadAllBytes(FileImageFolder + ImageList.SelectedValue));
            ResizeWidth.Text = img.Width.ToString();
            ResizeHeight.Text = img.Height.ToString();
            ImageAspectRatio.Value = "" + img.Width / (float)img.Height;

            int pos = ImageList.SelectedItem.Text.LastIndexOf('.');
            if (pos == -1)
                return;
            RenameImageButton.OnClientClick = "var name = prompt('Enter new filename:','" + ImageList.SelectedItem.Text.Substring(0, pos) + "'); if (name == null || name == '') return false; document.getElementById('" + NewImageName.ClientID + "').value = name + '" + ImageList.SelectedItem.Text.Substring(pos) + "';";
            OkButton.OnClientClick = "window.top.opener.CKEDITOR.dialog.getCurrent().setValueOf('info', 'txtUrl', encodeURI('" + ImageFolder + ImageList.SelectedValue.Replace("'", "\\'") + "')); window.top.close(); window.top.opener.focus();";
        }

        protected void RenameImage(object sender, EventArgs e)
        {
            string filename = UniqueFilename(NewImageName.Value);
            File.Move(FileImageFolder + ImageList.SelectedValue, FileImageFolder + filename);
            BindImageList();
            ImageList.SelectedValue = filename;
            SelectImage(null, null);
        }

        protected void DeleteImage(object sender, EventArgs e)
        {
            File.Delete(FileImageFolder + ImageList.SelectedValue);
            BindImageList();
            SelectImage(null, null);
        }

        protected void ResizeWidthChanged(object sender, EventArgs e)
        {
            float ratio = Util.Parse<float>(ImageAspectRatio.Value);
            int width = Util.Parse<int>(ResizeWidth.Text);

            ResizeHeight.Text = "" + (int)(width / ratio);
        }

        protected void ResizeHeightChanged(object sender, EventArgs e)
        {
            float ratio = Util.Parse<float>(ImageAspectRatio.Value);
            int height = Util.Parse<int>(ResizeHeight.Text);

            ResizeWidth.Text = "" + (int)(height * ratio);
        }

        protected void ResizeImage(object sender, EventArgs e)
        {
            int width = Util.Parse<int>(ResizeWidth.Text);
            int height = Util.Parse<int>(ResizeHeight.Text);

            ImageMedia img = ImageMedia.Create(File.ReadAllBytes(FileImageFolder + ImageList.SelectedValue));
            img.Resize(width, height);
            File.Delete(FileImageFolder + ImageList.SelectedValue);
            File.WriteAllBytes(FileImageFolder + ImageList.SelectedValue, img.ToByteArray());

            ResizeMessage.Text = "Image successfully resized!";
            SelectImage(null, null);
        }

        protected void Upload(object sender, EventArgs e)
        {
            if (IsImage(UploadedImageFile.FileName))
            {
                string filename = UniqueFilename(UploadedImageFile.FileName);
                UploadedImageFile.SaveAs(FileImageFolder + filename);

                byte[] data = ImageMedia.Create(UploadedImageFile.FileBytes).Resize(960, null).ToByteArray();
                FileStream file = File.Create(FileImageFolder + filename);
                file.Write(data, 0, data.Length);
                file.Close();

                BindImageList();
                ImageList.SelectedValue = filename;
                SelectImage(null, null);
            }
        }

        protected void Clear(object sender, EventArgs e)
        {
            Session.Remove("viewstate");
        }

        //util methods
        private bool IsImage(string file)
        {
            return file.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                file.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase) ||
                file.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase);
        }

        protected string UniqueFilename(string filename)
        {
            string newfilename = filename;

            for (int i = 1; File.Exists(FileImageFolder + newfilename); i++)
            {
                newfilename = filename.Insert(filename.LastIndexOf('.'), "(" + i + ")");
            }

            return newfilename;
        }

        protected string UniqueDirectory(string directoryname)
        {
            string newdirectoryname = directoryname;

            for (int i = 1; Directory.Exists(FileImageFolderRoot + newdirectoryname); i++)
            {
                newdirectoryname = directoryname + "(" + i + ")";
            }

            return newdirectoryname;
        }

        //properties
        protected string ImageFolderRoot
        {
            get { return ResolveUrl(ConfigurationManager.AppSettings["ImageRoot"]); }
        }

        protected string FileImageFolderRoot
        {
            get { return Server.MapPath(ImageFolderRoot); }
        }

        protected string ImageFolder
        {
            get { return ImageFolderRoot + ViewState["folder"]; }
            set { ViewState["folder"] = value; }
        }

        protected string FileImageFolder
        {
            get { return Server.MapPath(ImageFolder); }
        }
    }
}