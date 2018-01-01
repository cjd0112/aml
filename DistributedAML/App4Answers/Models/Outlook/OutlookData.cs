#region Copyright Syncfusion Inc. 2001-2017.
// Copyright Syncfusion Inc. 2001-2017. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Generic;

namespace App4Answers.Models.Outlook
{

    public class OutlookItem
    {

        public string ContactID { get; set; }

        public string CompanyName { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Greetings { get; set; }
        public string Message { get; set; }
        public string To { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Today { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string Size { get; set; }


    }

    public class OutlookData
    {
       

        public List<OutlookItem> OutlookItem { get; set; }

        public List<OutlookItem> OutlookItem1 { get; set; }

        public TreeViewInfo TreeviewDB { get; set; }

        public MenuInfo MenuDB { get; set; }

    }

    public class TreeViewInfo
    {
       
        public List<TreeviewData> TreeData { get; set; }

    }
    public class MenuInfo
    {
     

        public List<MenuDataSource> MenuData { get; set; }

    }

    public class TreeviewData
    {
        public int ID { get; set; }

        public int PID { get; set; }

        public string Name { get; set; }

        public bool HasChild { get; set; }

        public bool Expanded { get; set; }

        public string Count { get; set; }

    }

    public class MenuDataSource
    {
        public int ID { get; set; }

        public string ParentId { get; set; }

        public string Text { get; set; }

        public string Sprite { get; set; }
    }

   
}