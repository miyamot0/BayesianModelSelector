namespace BayesianModeling.ViewModel
{
    class ViewModelLicense : ViewModelBase
    {
        public string licenseTitle { get; set; }
        public string LicenseTitle
        {
            get { return licenseTitle; }
            set
            {
                licenseTitle = value;
                OnPropertyChanged("LicenseTitle");
            }
        }

        public string licenseText { get; set; }
        public string LicenseText
        {
            get { return licenseText; }
            set
            {
                licenseText = value;
                OnPropertyChanged("LicenseText");
            }
        }

        public ViewModelLicense()
        {

        }
    }
}
