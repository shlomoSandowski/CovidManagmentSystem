using CovidManagementSystem.Controllers;
namespace CovidManagementSystem.Entities
{

    public abstract class IPotentialPatient
    {
        public abstract string firstName { get; init; }
        public abstract string lastName { get; init; }
        public abstract string phoneNumber { get; init; }
        public abstract void AddLabResults(LabTestResult result);
        public abstract List<LabTestResult> GetLabResults();
        public abstract bool IsCovidPositive();
        public abstract bool IsIsolated();
        public abstract string GetInfectedByPatientID();  
        public abstract DateTime GetIsolationDate();
        
    }
    public abstract class Decorator : IPotentialPatient
    {
        protected IPotentialPatient _patient;
        public Decorator(IPotentialPatient patient)
        {
            this._patient = patient;
        }
    }
    public class Patient: Decorator
    {
        public string patientID { get; init; } //Service responsability
        public string govtID { get; init; }
        public override string firstName { get => _patient.firstName;init { }}
        public override string lastName { get => _patient.lastName; init { } }
        public DateTime birthDate { get; set; }
        public override string phoneNumber { get => _patient.phoneNumber; init { } }
        public string? email { get; set; }
        public Address address { get; set; }
        public int houseResidentsAmount { get; set; }
        public string? infectedByPatientID { get => _patient.GetInfectedByPatientID();set { } }
        //if patient wasn't suspect isolation date starts with registration
        List<NewPatientVisitedSite> _route { get; set; } = new List<NewPatientVisitedSite>();
        List<PotentialPatient> _encounters { get; set; } = new List<PotentialPatient>();
       
       

        public Patient(PotentialPatient baseDetails,string govtId,DateTime birthDate, string email, 
            Address address,int houseResidentsAmount)
            :base(baseDetails)
        {
            this.patientID = baseDetails.potentialPatientID;
            this.govtID = govtId;
            this.birthDate = birthDate;
            this.email = email;
            this.houseResidentsAmount = houseResidentsAmount;
            this.address = address;
        }
     
        public override void AddLabResults(LabTestResult result)
        {
            this._patient.AddLabResults(result);
        }
        public override List<LabTestResult> GetLabResults() {
            return this._patient.GetLabResults();
        }
        public override bool IsCovidPositive()
        {
            return this._patient.IsCovidPositive();
        }
        public override bool IsIsolated() => _patient.IsIsolated();
        public override DateTime GetIsolationDate() => _patient.GetIsolationDate();
        public override string GetInfectedByPatientID() => _patient.GetInfectedByPatientID();
        public void AddPatientVisitedSite(NewPatientVisitedSite site)
        {
            _route.Add(site);
        }
        public List<NewPatientVisitedSite> GetPatientRoute()
        {
            return _route;
        }
        public void AddPatientEncounter(PotentialPatient encounter)
        {
            _encounters.Add(encounter);
        }
        public List<PotentialPatient> GetPatientEncounters()
        {
            return _encounters;
        }
        public PatientMedicalFile GetMedicalFile()
        {
            return new PatientMedicalFile( this, IsCovidPositive(), GetLabResults());
        }
        public PotentialPatient GetAsPotential()
        {
            return (PotentialPatient)_patient;
        }
    }
    public class PotentialPatient : IPotentialPatient
    {

        Guid _patientID { get; init; } = Guid.NewGuid();
        public string potentialPatientID { get => _patientID.ToString(); init { } }
        private string _firstName { get; set; }
        public override string firstName { get => this._firstName; init { } }
        private string _lastName { get; set; }
        public override string lastName { get => _lastName; init { } }
        private string _phoneNumber { get; set; }
        public override string phoneNumber { get => _phoneNumber; init { } }
        DateTime _isolationDate { get; set; }
        bool _isCovidPositive { get; set; } = false;
        bool _isIsolated { get; set; }
        string _infectedByPatientId { get; set; }
        List<LabTestResult> _labs { get; set; }

        public PotentialPatient(string firstName, string lastName, string phoneNumber, string infectedByPatientID)
        {
            this._firstName = firstName;
            this._lastName = lastName;
            this._phoneNumber = phoneNumber;
            _isolationDate = DateTime.Now;
            _labs = new List<LabTestResult>();
            _isCovidPositive = false;
            _infectedByPatientId = infectedByPatientID;
            _isIsolated = true;
        }
        public PotentialPatient(string firstName, string lastName, string phoneNumber, string infectedByPatientID=null,bool isCovidPositive=true)
            :this(firstName, lastName, phoneNumber, infectedByPatientID)
        {
            _isCovidPositive = isCovidPositive;
            _infectedByPatientId = infectedByPatientID;
            _isIsolated = true;
        }
        public override void AddLabResults(LabTestResult result)
        {
            //assuming that lab results can come in not ordered by date
            _labs.Add(result);
            var orderedLab = _labs.OrderByDescending(lab => lab.testDate);
            _isCovidPositive = orderedLab.First().isCovidPositive;

            //is covid positive
            if (_isCovidPositive)
                _isIsolated = _isCovidPositive;
            //if is isolated check if release
            else if (_isIsolated && (DateTime.Now - _isolationDate).Days >= 14 && _labs.Count() > 2 &&  !orderedLab.ElementAt(1).isCovidPositive)
                     _isIsolated = false;
                     
        }
        public override List<LabTestResult> GetLabResults()
        {
            return _labs;
        }
        public override bool IsCovidPositive() =>_isCovidPositive; 
        public override bool IsIsolated() =>_isIsolated; 
        public override DateTime GetIsolationDate() => _isolationDate; 
        public override string GetInfectedByPatientID() => _infectedByPatientId;

    }
    public class PatientEncounter
    {
        public PotentialPatient potentialPatientDetails { get; set; }
        public Patient? encounteredPatient{ get; set; }
        public PatientEncounter(PotentialPatient patientDetails, Patient encounteredPatient)
        {
            this.potentialPatientDetails = patientDetails;
            this.encounteredPatient = encounteredPatient;
        }
        
    }    
    public class PatientManager
    {
        //hold all registered patients
        public List<Patient> patients { get; init; } = new List<Patient>();
        //supports adding new patients
        public NewPatientResponse AddPatient(NewPatientRequest newPatient)
        {
            if (isExist(newPatient.govtID))
                throw new Exception($"patient with govtID {newPatient.govtID} is already in system");
            Patient toAdd = new Patient(new PotentialPatient(newPatient.firstName, newPatient.lastName, newPatient.phoneNumber),
                newPatient.govtID, newPatient.birthDate, newPatient.email, newPatient.address, newPatient.houseResidentAmount);
            patients.Add(toAdd);
            return new NewPatientResponse{ patientID = toAdd.patientID };
        }
        public NewPatientResponse AddPatient(PotentialPatient baseDetails, string govId, DateTime birthDate, string email,
            Address address, int houseResidentsAmount)
        {
            Patient toAdd = new Patient(baseDetails, govId, birthDate, email, address, houseResidentsAmount);
            patients.Add(toAdd);
            return new NewPatientResponse { patientID = toAdd.patientID };
        }
        //supports getting all patients
        public List<Patient> GetPatients()
        {
            return patients;
        }
        //supports adding site to patients route
        public void AddPatientVisitedSite(string id,NewPatientVisitedSite site)
        {
           GetPatient(id).AddPatientVisitedSite(site);   
        }
        //supports getting patients route  
        public List<NewPatientVisitedSite> GetPatientRoute(string id)
        {
            return  GetPatient(id).GetPatientRoute();  
        }
        //supports adding new patients encounter 
        public void AddPatientEncounter(string id,PotentialPatient encounter)
        {
            GetPatient(id).AddPatientEncounter(encounter);
        }
        //supports returning patients encounters 
        public List<PatientEncounter> GetPatientEncounters(string id)
        {
            var patient = GetPatient(id);
            var potentialPatients = patient.GetPatientEncounters();
            var encounters = new List<PatientEncounter>();
            foreach (var potentialPatient in potentialPatients)
                encounters.Add(new PatientEncounter(potentialPatient, patient));
            return encounters;

        }
        //supports getting full patient (assuming only registered patients have medical file)
        public PatientMedicalFile GetMedicalFile(string id)
        {
            return GetPatient(id).GetMedicalFile();
        }
        public List<PatientEncounter> GetNewSick(DateTime since)
        {
            Patient encountered = null;
            var newSickPatients = patients.Where(patient => patient.IsCovidPositive() && patient.GetIsolationDate() >= since);
            List <PatientEncounter> newSick = new List<PatientEncounter>();
            foreach (Patient patient in newSickPatients) {
                if (patient.GetInfectedByPatientID() != null)
                    encountered = GetPatient(patient.GetInfectedByPatientID());
                else encountered = null;
                newSick.Add(new PatientEncounter(patient.GetAsPotential(), encountered));
            }
            return newSick;
        }

        public List<PatientEncounter> GetIsolatedPatients()
        {
            Patient encountered = null;
            var patients = this.patients.Where(patient => patient.IsIsolated());
            List<PatientEncounter> asEncounterd = new List<PatientEncounter>();
            foreach (Patient patient in patients)
            {
                if (patient.GetInfectedByPatientID() != null)
                    encountered = GetPatient(patient.GetInfectedByPatientID());
                else encountered = null;
                asEncounterd.Add(new PatientEncounter( patient.GetAsPotential(), encountered));
            }
            return asEncounterd;
        }
        // adds lab result to patient 
        public NewPatientResponse AddLabResult(LabTestResult lab)
        {
            GetPatient(lab.patientID).AddLabResults(lab);
            return new NewPatientResponse { patientID = lab.patientID };
        }
        //gets patient from patients by id
        public Patient GetPatient(string id)
        {
            var patient = patients.FirstOrDefault(patient => patient.patientID == id);
            if (patient == null)
                throw (new Exception($"No patient with {id} id"));
            return patient;
        }
        //checks if patient with given id exists in database (assuming only govrID and patientID must be unique)
        public bool isExist(string govrID)
        {
            return patients.Any(patients=>patients.govtID == govrID);
        }
    }
}





