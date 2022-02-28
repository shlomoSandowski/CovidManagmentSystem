namespace CovidManagementSystem.Entities
{
    public class LabTestResult
    {
        public string labID { get; init; }
        public string testID { get; init; }
        public string patientID { get; set; }
        public DateTime testDate { get; set; }
        public bool isCovidPositive { get; set; }

        public LabTestResult() { }

        public LabTestResult(string labID,string testIDstring ,string patientId, DateTime testDate,bool isCovidPositive)
        {
            this.labID = labID; 
            this.testID = testIDstring; 
            this.patientID = patientId;
            this.testDate = testDate;
            this.isCovidPositive = isCovidPositive;
        }

    }
}
