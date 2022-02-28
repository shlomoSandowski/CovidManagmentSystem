using CovidManagementSystem.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CovidManagementSystem.Controllers
{


    [Route("/[controller]")]
    [ApiController]
    public class CovidMgmtSystemController : ControllerBase
    {

        private readonly ICovidSystemManager Manager;
        public CovidMgmtSystemController(ICovidSystemManager manager)
        {
            Manager = manager;
        }

        //Add a new patient to be tracked by the system
        [HttpPut("/patients")]
        public ActionResult<NewPatientResponse> AddPatient([FromBody,SwaggerRequestBody(Required=true)] NewPatientRequest newPatient)
        {
            try
            {
                if(ModelState.IsValid)
                    return Ok(Manager.AddPatient(newPatient));
                return BadRequest(ModelState);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        //Returns the list of all the patients tracked by the system(only real patients and not potential ones)
        [HttpGet("/patients")]
        public ActionResult<IEnumerable<Patient>> GetPatients()
        {
            return Ok(Manager.GetPatients());
        }

        //Add a location a patient visited during the last 7 days
        [HttpPut("/patients/{id}/route")]
        public ActionResult<NewPatientEncounterResponse> AddSiteToRoute(string id, [FromBody, SwaggerRequestBody(Required = true)] NewPatientVisitedSite site)
        {
            try
            {
                if (site.dateOfVisit <= DateTime.Now && site.dateOfVisit >= DateTime.Now.AddDays(-7))
                {
                    Manager.AddPatientRoute(id, site);
                    return Ok(new NewPatientEncounterResponse());
                }

                return BadRequest("invalid dates");
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //Returns the list of all the locations the patient visited in during the last 7 days
        [HttpGet("/patients/{id}/route")]
        public ActionResult<List<NewPatientVisitedSite>> GetPatientRoute(string id)
        {
            try { 
                return Ok(Manager.GetPatientRoute(id).Where(site=>
                (site.dateOfVisit <= DateTime.Now && site.dateOfVisit >= DateTime.Now.AddDays(-7))).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Add the details of a person the patient met during the last 7 days 
        [HttpPut("/patients/{id}/encounter")]
        public ActionResult<NewPatientEncounterResponse> AddEncounter(string id,[FromBody, SwaggerRequestBody(Required = true)] NewPatientEncounter encounter)
        {
            try {
                if(Manager.GetIsolationDate(id) >= DateTime.Now.AddDays(-7)){
                    Manager.AddPatientEncounter(id, encounter);
                    return Ok(new NewPatientEncounterResponse());
                }
                return BadRequest("invalid date");
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Return the list of the people the patient met during the last 7 days
        [HttpGet("/patients/{id}/encounter")]
        public ActionResult<List<PatientEncounter>> GetEncounters(string id)
        {
            try
            {
                return Ok( Manager.GetPatientEncounters(id).Where(encounter =>
                (encounter.potentialPatientDetails.GetIsolationDate() <= DateTime.Now &&
                encounter.potentialPatientDetails.GetIsolationDate() >= DateTime.Now.AddDays(-7))).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Returns the person details and whether he is sick or not together with all his/her lab tests
        [HttpGet("/patients/{id}/full")]
        public ActionResult<PatientMedicalFile> GetFullPatient(string id)
        {
            try
            {
                return Ok(Manager.GetPatient(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //will display a list of all sick people who were added after the value of 'since'
        [HttpGet("/patients/new")]
        public ActionResult<List<PatientEncounter>> GetNewSick([FromQuery] DateTime dateTime)
        {
             return Ok(Manager.GetNewSick(dateTime));
        }

        //Returns the list of encounters where the person details were not inserted yet
        [HttpGet("/patients/potential")]
        public ActionResult<List<PatientEncounter>> GetPotentialPatient()
        {
            return Ok(Manager.GetPotentialPatients());
        }

        //Returns the list of all the people in the system that are in isolation (person is isolated until he 
        //has two negative tests since he encountered an infected person or reported infected)
        [HttpGet("/patients/isolated")]
        public ActionResult<List<PatientEncounter>> GetIsolated()
        {
            return Ok(Manager.GetIsolated());
        }

        //Removed the potential patient and transform him into real patient
        [HttpPost("/patients/potential/{potentialPatientId}")]
        public ActionResult<NewPatientResponse> CreatePatient(string potentialPatientId, [FromBody, SwaggerRequestBody(Required = true)] NewPatientRequest newPatient)
        {
            try
            {
                return Ok(Manager.UpdatePatient(potentialPatientId, newPatient));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //Add a lab test result
        [HttpPost("/labtests")]
        public ActionResult<NewPatientResponse> AddLabTest([FromBody, SwaggerRequestBody(Required = true)] LabTestResult labTest)
        {
            try
            {
                return Ok(Manager.AddLabTest(labTest));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Returns statistics about the current state –amount of sicks, amount of isolated,
        //have healed, and how many sick we have per city
        [HttpGet("/statistics")]
        public ActionResult<CovidStatistics> GetStatistics()
        {
            return Ok(Manager.GetStatistic());
        }

    }
        
        public class NewPatientResponse
        {
            public string? patientID { get; set; }
        } 
        public class NewPatientEncounterResponse {}
        public class NewPatientEncounter
        {
            [Required]
            public string firstName { get; set; }
            [Required]  
            public string lastName { get; set; }
            [Required]
            public string phoneNumber { get; set; }
        }
        public class NewPatientRequest
        {
        [Required,]
        public string govtID { get; init; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName {get; set; }
        [Required]
        public DateTime birthDate { get; set; }
        [Required,Phone]
        public string phoneNumber { get; set; }
        [EmailAddress]
        public string? email { get; set; }
        [Required]
        public Address address { get; set; }
        [Required]
        public int houseResidentAmount { get; set; }
        [Required,  Range(0,1)]
        public bool isCovidPositive { get; set; }
        }




}


