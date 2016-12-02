using System.Collections.Generic;
using NUnit.Framework;

using CMS.Tests;


namespace Kentico.Web.Mvc.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class FeatureSetTests
    {
        private TestFeature testFeature;
        private FeatureSet featureSet;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            testFeature = new TestFeature();                     
        }


        [SetUp]
        public void Setup()
        {
            featureSet = new FeatureSet();
        }


        [Test]
        [Description("Test whether GetFeature returns existing feature correctly.")]
        public void GetFeature_Existing_ReturnsExistingFeature()
        {                    
            featureSet.SetFeature<TestFeature>(testFeature);
           
            Assert.AreEqual(testFeature, featureSet.GetFeature<TestFeature>(), "GetFeature didn't return existing feature correctly.");
        }


        [Test]
        [Description("Test whether GetFeature returns null when feature does not exist.")]
        public void GetFeature_NonExisting_ReturnsNull()
        {           
            Assert.AreEqual(null, featureSet.GetFeature<TestFeature>(), "Non-existing feature didn't return null.");
        }


        [Test]
        [Description("Test whether SetFeature updates original feature correctly.")]
        public void SetFeature_UpdateExisting_ReturnsUpdatedFeature()
        {
            var alternativeFeature = new TestFeature();

            featureSet.SetFeature<TestFeature>(testFeature);
            featureSet.SetFeature<TestFeature>(alternativeFeature);

            var retrievedFeature = featureSet.GetFeature<TestFeature>();

            CMSAssert.All(
                () => Assert.AreEqual(alternativeFeature, retrievedFeature, "SetFeature didn't update the original feature."),
                () => Assert.AreNotEqual(testFeature, retrievedFeature, "Retrieved feature was the same as original feature.")
            );
        }
        

        [Test]
        [Description("Test whether RemoveFeature removes feature of specified type correctly.")]
        public void RemoveFeature_Existing_CorrectlyRemovesFeature()
        {            
            featureSet.SetFeature<TestFeature>(testFeature);

            featureSet.RemoveFeature<TestFeature>();            

            Assert.AreEqual(null, featureSet.GetFeature<TestFeature>(), "Retrieved feature should be null after remove.");            
        }


        private class TestFeature
        {

        }
    }    
}
