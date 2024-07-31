#region Copyright Syncfusion Inc. 2001-2019.
// Copyright Syncfusion Inc. 2001-2019. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
//using OrganizationLayout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutomaticLayout_OrganizationLayout
{
    public class Employee : INotifyPropertyChanged
    {
        private string id;
        private string heatmap = "#FFC34444";
        private string parentId;
        private bool isRoot;
        private State isexpand;

        public Employee()
        {
            Models = new ObservableCollection<Employee>();
        }
        
        private string _mDesignation;
        public string Designation
        {
            get { return _mDesignation; }
            set
            {
                if (_mDesignation != value)
                {
                    _mDesignation = value;
                    OnPropertyChanged("Designation");
                }
            }
        }       

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(("Id"));
                }
            }
        }

        public string ParentId
        {
            get
            {
                return parentId;
            }
            set
            {
                if (parentId != value)
                {
                    parentId = value;
                    OnPropertyChanged(("ParentId"));
                }
            }
        }
              

        public string RatingColor
        {
            get
            {
                return heatmap;
            }
            set
            {
                if (heatmap != value)
                {
                    if (value != null)
                    {
                        heatmap = value;
                        OnPropertyChanged(("RatingColor"));
                    }
                }
            }
        }
        public bool IsRoot
        {
            get
            {
                return isRoot;
            }
            set
            {
                if (isRoot != value)
                {
                    isRoot = value;
                    OnPropertyChanged(("IsChild"));
                }
            }
        }
        public State IsExpand
        {
            get
            {
                return isexpand;
            }
            set
            {
                if (isexpand != value)
                {
                    isexpand = value;
                    OnPropertyChanged(("IsExpand"));
                }
            }
        }

        private ObservableCollection<Employee> models;


        public ObservableCollection<Employee> Models
        {
            get { return models; }
            set
            {
                models = value;
                OnPropertyChanged(("Models"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public enum State
    {
        Expand,
        Collapse,
        None
    };

    public class Employees : ObservableCollection<Employee>
    {

    }
}
