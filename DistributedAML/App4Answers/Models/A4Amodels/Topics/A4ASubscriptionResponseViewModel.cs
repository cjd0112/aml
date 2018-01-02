using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App4Answers.Models.A4Amodels.Base;

namespace App4Answers.Models.A4Amodels.Topics
{
    public class LoadData
    {
        public int Id { get; set; }
        public int Parent { get; set; }
        public string Text { get; set; }
        public string SpriteImage { get; set; }
        public string ImageURL { get; set; }
        public bool HasChild { get; set; }
        public bool Expanded { get; set; }
        public bool Selected { get; set; }
        public bool NodeChecked { get; set; }
        public object NodeProperty { get; set; }
        public object LinkProperty { get; set; }
        public object ImageProperty { get; set; }
    }



    public class A4ASubscriptionResponseViewModel : IViewModel
	{
	    void AddNextNode(ref int cnt, int parentId, SubscriptionNode r, List<LoadData> lst)
	    {
	        var foo = (new LoadData
	        {
	            Id = cnt++,
	            Parent = parentId,
	            Text = r.Name,
	            
	        });
	        lst.Add(foo);
	        if (r.Children.Count == 0)
	            foo.HasChild = false;
	        else
	            foo.HasChild = true;
	        foreach (var c in r.Children)
	        {
	            AddNextNode(ref cnt, foo.Id, c, lst);
	        }
	    }

        public A4ASubscriptionResponseViewModel ()
		{
		}

        public SubscriptionResponse VM { get; set; }
	    public A4ASubscriptionResponseViewModel(SubscriptionResponse vm) 
	    {
	        VM = vm;
	    }


	    public List<LoadData> LoadData
	    {
	        get
	        {
	            var z = new List<LoadData>();
	            int cnt = 0;
	            AddNextNode(ref cnt, -1, VM.Root, z);
	            return z;
	        }
	    }



	    public ModelNames.ObjectTypes ObjectTypes { get; set; }
	    public ModelNames.Verb Verb { get; set; }
	    public ModelNames.ActionNames ActionNames { get; set; }
	}
}