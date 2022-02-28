using Microsoft.VisualStudio.TestTools.UnitTesting;
using CovidManagementSystem.Controllers;
using CovidManagementSystem.Entities;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using System;
using System.Web.Http.Results;
using OkResult = Microsoft.AspNetCore.Mvc.OkResult;
using System.Collections;
using BadRequestResult = System.Web.Http.Results.BadRequestResult;

namespace CovidManagementSystem.UnitTest
{
    [TestClass]
    public class CovidMgmtSystemControllerTest
    {

        [TestMethod]
        public void AddPatient_ValidInput_ValidResponse()
        {
            //setup
            Address address = new Address("city", "street", 0, 0);
            NewPatientRequest patient = new NewPatientRequest
            {
                govtID = "12",
                firstName = "name",
                lastName = "lastName",
                phoneNumber = "123",
                email = "sad@as.com",
                birthDate = System.DateTime.UtcNow,
                address = address,
                houseResidentAmount = 0,
                isCovidPositive = true
            };
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);

            //act
            var result = controller.AddPatient(patient);

            //assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<NewPatientResponse>));
        }

        [TestMethod]
        public void AddPatient_InvalidInput_BadRequestResponse()
        {
            //setup
            Address address = new Address("city", "street", 0, 0);
            NewPatientRequest wrongsyntax = new NewPatientRequest
            {
                govtID = "23aq",
                firstName = "name23",
                lastName = "lastName23",
                phoneNumber = "a2342sdd",
                email = "sada3com",
                birthDate = System.DateTime.UtcNow,
                address = address,
                houseResidentAmount = 0,
                //patient is positive when inserted
                isCovidPositive = false
            };
            NewPatientRequest missingFields = new NewPatientRequest
            {
                govtID = "23",
                firstName = "name",
                lastName = "lastName",
                phoneNumber = "123",
                email = "sadas.com",
                address = address,
                houseResidentAmount = 0,
                isCovidPositive = true
            };
            NewPatientRequest nullBody = new NewPatientRequest { };
           

            //act
            var result1 = Validator.TryValidateObject(wrongsyntax, new ValidationContext(wrongsyntax), null, true);
            var result2 = Validator.TryValidateObject(missingFields, new ValidationContext(missingFields), null, true);
            var result3 = Validator.TryValidateObject(nullBody, new ValidationContext(nullBody), null, true);

            // Assert
            Assert.IsFalse(result1, "Expected validation to fail.");
            Assert.IsFalse(result2, "Expected validation to fail.");
            Assert.IsFalse(result3, "Expected validation to fail.");
        }
        [TestMethod]
        public void GetPatient_ValidCall_ValidResponseType()
        {
            //setup
            var repo = new Mock<ICovidSystemManager>();
            repo.Setup(x => x.GetPatients()).Returns(new List<Patient>());
            var controller = new CovidMgmtSystemController(repo.Object);

            //act
            var result = controller.GetPatients();

            //assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<Patient>>));
        }

        [TestMethod]
        public void AddSiteToRoute_BadDates_ValidResponseType()
        {
            //setup
            Address address = new Address("city", "street", 0, 0);
            NewPatientRequest patient = new NewPatientRequest
            {
                govtID = "12",
                firstName = "name",
                lastName = "lastName",
                phoneNumber = "123",
                email = "sad@as.com",
                birthDate = System.DateTime.UtcNow,
                address = address,
                houseResidentAmount = 0,
                isCovidPositive = true
            };
            NewPatientVisitedSite site = new NewPatientVisitedSite
            {
                dateOfVisit = DateTime.MinValue,
                siteName = "dsfs",
                siteAddress = address
            };
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);


            //act
            var badResult = controller.AddSiteToRoute("test", site);

            // Assert
            Assert.IsNotInstanceOfType(badResult, typeof(OkResult));
        }
        [TestMethod]
        public void AddSiteToRoute_InvalidInput_ValidationError()
        {
            //setup
            Address address = new Address("city", "street", 0, 0);
            NewPatientVisitedSite site = new NewPatientVisitedSite
            {
                dateOfVisit = DateTime.Now,
                siteAddress = address
            };
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);

            //act
            var result = controller.AddSiteToRoute("test", site);

            // Assert
            Assert.IsInstanceOfType(result,typeof(ActionResult<List<NewPatientVisitedSite>>));
        }
        [TestMethod]
        public void AddPatientRoute_InvalidRoute_BadResponse()
        {
            //setup
            Address address = new Address("city", "street", 0, 0);
            NewPatientVisitedSite site = new NewPatientVisitedSite
            {
                dateOfVisit = DateTime.MinValue,
                siteAddress = address
            };
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);

            //act
            var result = controller.AddSiteToRoute("test", site);

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(OkResult));
        }
        [TestMethod]
        public void AddEncounter_InvalidInput_ValidationError()
        {
            //setup
            NewPatientEncounter encounter = new NewPatientEncounter
            {
                firstName = "bla",
                lastName = "blabla"
            };
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);

            //act
            var result = Validator.TryValidateObject(encounter, new ValidationContext(encounter), null, true);
            // Assert
            Assert.IsFalse(result, "Expected validation to fail."); 
        }
        [TestMethod]
        public void AddEncounter_ValidInput_Valid()
        {
            //setup
            NewPatientEncounter encounter = new NewPatientEncounter
            {
                firstName = "bla",
                lastName = "blabla",
                phoneNumber ="12312"
            };

            //act
            var result = Validator.TryValidateObject(encounter, new ValidationContext(encounter), null, true);
            // Assert
            Assert.IsTrue(result, "Expected validation to pass.");
        }
        [TestMethod]
        public void AddEncounter_ValidInput_ValidResponseType()
        {
            //setup
            NewPatientEncounter encounter = new NewPatientEncounter
            {
                firstName = "bla",
                lastName = "blabla",
                phoneNumber = "12312"
            };
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
            repo.Setup(x => x.AddPatientEncounter("test",encounter));


            //act
            var result = controller.AddEncounter("test", encounter);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<NewPatientEncounterResponse>));
        }
        [TestMethod]
        public void GetEncounters_inValidInput_errorResponse()
        {
            //setup
           
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
           


            //act
            var result = controller.GetEncounters("test");

            // Assert
            Assert.IsNotInstanceOfType(result, typeof(OkResult));
        }
        [TestMethod]
        public void GetFullPatient_ValidInput_ValidResponseType()
        {
            //setup
            Address address = new Address("city", "street", 0, 0);
            Patient patient = new Patient(new PotentialPatient("sdf", "sdf", "sdf", "sf"), "12", System.DateTime.UtcNow, "lastName", address, 0);
                
            PatientMedicalFile test = new PatientMedicalFile(patient, false, new List<LabTestResult>());
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
            repo.Setup(x => x.GetPatient("test")).Returns(test);


            //act
            var result = controller.GetFullPatient("test");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<PatientMedicalFile>));
        }

        [TestMethod]
        public void GetNewSick_ValidInput_ValidResponseType()
        {
            //setup
            var time = DateTime.Now;
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
            repo.Setup(x => x.GetNewSick(time)).Returns(new List<PatientEncounter>());


            //act
            var result = controller.GetNewSick(time);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<List<PatientEncounter>>));
        }

        [TestMethod]
        public void GetPotentialPatient_ValidInput_ValidResponseType()
        {
            //setup
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
            repo.Setup(x => x.GetPotentialPatients()).Returns(new List<PatientEncounter>());


            //act
            var result = controller.GetPotentialPatient();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<List<PatientEncounter>>));
        }
        [TestMethod]
        public void GetIsolated_ValidInput_ValidResponseType()
        {
            //setup
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
            repo.Setup(x => x.GetIsolated()).Returns(new List<PatientEncounter>());


            //act
            var result = controller.GetIsolated();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<List<PatientEncounter>>));
        }
        [TestMethod]
        public void CreatePatient_ValidInput_ValidResponseType()
        {
            //setup
            Address address = new Address("city", "street", 0, 0);
            NewPatientRequest patient = new NewPatientRequest
            {
                govtID = "12",
                firstName = "name",
                lastName = "lastName",
                phoneNumber = "123",
                email = "sad@as.com",
                birthDate = System.DateTime.UtcNow,
                address = address,
                houseResidentAmount = 0,
                isCovidPositive = true
            };
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
            repo.Setup(x => x.UpdatePatient("test",patient)).Returns(new NewPatientResponse());


            //act
            var result = controller.CreatePatient("test", patient);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<NewPatientResponse>));
        }
        [TestMethod]
        public void AddLabTest_ValidInput_ValidResponseType()
        {
            //setup
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
            repo.Setup(x => x.AddLabTest( new LabTestResult())).Returns(new NewPatientResponse());


            //act
            var result = controller.AddLabTest( new LabTestResult());

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<NewPatientResponse>));
        }
        [TestMethod]
        public void GetStatistics_ValidInput_ValidResponseType()
        {
            //setup
            var repo = new Mock<ICovidSystemManager>();
            var controller = new CovidMgmtSystemController(repo.Object);
            repo.Setup(x => x.GetStatistic()).Returns(new CovidStatistics());


            //act
            var result = controller.GetStatistics();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<CovidStatistics>));
        }
    }
}