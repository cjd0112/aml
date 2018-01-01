using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using App4Answers.Models.Outlook;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App4Answers.Controllers
{
    [Produces("application/json")]
    [Route("api/Mail")]
    public class MailController : Controller
    {
        private IHostingEnvironment env;
        public MailController(IHostingEnvironment env)
        {
            this.env = env;
        }


        [AcceptVerbs("GET")]
        public OutlookData LoadData()
        {
            XmlDocument doc = new XmlDocument();
            string path = Path.Combine(env.WebRootPath, "data", "data.xml");
            doc.Load(path);

            List<OutlookItem> outlist = new List<OutlookItem>();
            List<OutlookItem> outlist1 = new List<OutlookItem>();
            XmlNodeList list = doc.DocumentElement.SelectNodes("/root/Customers");
            int count = list.Count - 7;

            int index = 0;
            foreach (XmlNode node1 in list)
            {
                if (count != -1 && index >= count)
                {
                    OutlookItem txt1 = new OutlookItem();
                    txt1.ContactID = node1.SelectSingleNode("ContactID").InnerText;
                    txt1.CompanyName = node1.SelectSingleNode("CompanyName").InnerText;
                    txt1.ContactName = node1.SelectSingleNode("ContactName").InnerText;
                    txt1.ContactTitle = node1.SelectSingleNode("ContactTitle").InnerText;
                    txt1.Greetings = node1.SelectSingleNode("Greetings").InnerText;
                    txt1.Message = node1.SelectSingleNode("Message").InnerText;
                    txt1.To = node1.SelectSingleNode("To").InnerText;
                    txt1.Address = node1.SelectSingleNode("Address").InnerText;
                    txt1.City = node1.SelectSingleNode("City").InnerText;
                    txt1.PostalCode = node1.SelectSingleNode("PostalCode").InnerText;
                    txt1.Today = node1.SelectSingleNode("Today").InnerText;
                    txt1.Time = node1.SelectSingleNode("Time").InnerText;
                    txt1.Date = node1.SelectSingleNode("Date").InnerText;
                    txt1.Day = node1.SelectSingleNode("Day").InnerText;
                    txt1.Size = node1.SelectSingleNode("Size").InnerText;
                    outlist1.Add(txt1);
                }

                if (index != 3 && index != 4 && index != 5 && index != 6 && index != 7 && index != 8 && index != 9 &&
                    index != 10)
                {
                    OutlookItem txt = new OutlookItem();
                    txt.ContactID = node1.SelectSingleNode("ContactID").InnerText;
                    txt.CompanyName = node1.SelectSingleNode("CompanyName").InnerText;
                    txt.ContactName = node1.SelectSingleNode("ContactName").InnerText;
                    txt.ContactTitle = node1.SelectSingleNode("ContactTitle").InnerText;
                    txt.Greetings = node1.SelectSingleNode("Greetings").InnerText;
                    txt.Message = node1.SelectSingleNode("Message").InnerText;
                    txt.To = node1.SelectSingleNode("To").InnerText;
                    txt.Address = node1.SelectSingleNode("Address").InnerText;
                    txt.City = node1.SelectSingleNode("City").InnerText;
                    txt.PostalCode = node1.SelectSingleNode("PostalCode").InnerText;
                    txt.Today = node1.SelectSingleNode("Today").InnerText;
                    txt.Time = node1.SelectSingleNode("Time").InnerText;
                    txt.Date = node1.SelectSingleNode("Date").InnerText;
                    txt.Day = node1.SelectSingleNode("Day").InnerText;
                    txt.Size = node1.SelectSingleNode("Size").InnerText;
                    outlist.Add(txt);
                }

                index++;
            }

            TreeViewInfo data = new TreeViewInfo { TreeData = treeData() };
            MenuInfo data1 = new MenuInfo { MenuData = menuData() };
            return new OutlookData
            {
                OutlookItem = outlist,
                OutlookItem1 = outlist1,
                TreeviewDB = data,
                MenuDB = data1
            };
        }


        private List<TreeviewData> treeData()
        {
            List<TreeviewData> treedata = new List<TreeviewData>();
            treedata.Add(new TreeviewData() { ID = 1, Name = "Favorites", HasChild = true, Expanded = true });
            treedata.Add(new TreeviewData() { ID = 2, PID = 1, Name = "Inbox", Count = "3" });
            treedata.Add(new TreeviewData() { ID = 3, PID = 1, Name = "Clutter", Count = "3" });
            treedata.Add(new TreeviewData() { ID = 4, PID = 1, Name = "Sent Items", Count = "" });
            treedata.Add(new TreeviewData() { ID = 5, PID = 1, Name = "Drafts", Count = "3" });
            treedata.Add(new TreeviewData() { ID = 6, Name = "Andrew Fuller", HasChild = true, Expanded = true });
            treedata.Add(new TreeviewData() { ID = 7, PID = 6, Name = "Inbox", Count = "3" });
            treedata.Add(new TreeviewData() { ID = 8, PID = 6, Name = "Clutter", Count = "3" });
            treedata.Add(new TreeviewData() { ID = 9, PID = 6, Name = "Drafts", Count = "3" });
            treedata.Add(new TreeviewData() { ID = 10, PID = 6, Name = "Sent Items", Count = "" });
            return treedata;
        }

        private List<MenuDataSource> menuData()
        {
            List<MenuDataSource> menudata = new List<MenuDataSource>();
            menudata.Add(new MenuDataSource() { ID = 1, Text = "New", ParentId = null, Sprite = "ej-icon-add" });
            menudata.Add(new MenuDataSource()
            {
                ID = 2,
                Text = "Delete",
                ParentId = null,
                Sprite = "ej-icon-trash-can1-wf"
            });
            menudata.Add(new MenuDataSource()
            {
                ID = 3,
                Text = "Archive",
                ParentId = null,
                Sprite = "ej-icon-archive02-wf"
            });
            menudata.Add(new MenuDataSource() { ID = 4, Text = "Junk", ParentId = null });
            menudata.Add(new MenuDataSource() { ID = 5, Text = "Sweep", ParentId = null });
            menudata.Add(new MenuDataSource() { ID = 6, ParentId = "1", Text = "Email message" });
            menudata.Add(new MenuDataSource() { ID = 7, ParentId = "1", Text = "Calendar event" });
            menudata.Add(new MenuDataSource() { ID = 8, ParentId = "4", Text = "Junk" });
            menudata.Add(new MenuDataSource() { ID = 9, ParentId = "4", Text = "Phishing" });
            return menudata;
        }
    }
}