using Domain.ValueObject;
using UseCase.DTO;

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests.EditMode.UseCases
{
    public sealed class LicenseDataMapperTest
    {
        private License _validLicense;
        private IReadOnlyList<License> _validLicenseList;

        [SetUp]
        public void SetUp()
        {
            // Create valid test license
            _validLicense = new License(
                "Test License Title",
                "This is a test license text content.",
                "https://example.com/license"
            );

            // Create valid license list
            _validLicenseList = new List<License>
            {
                new License("License 1", "Content 1", "https://example1.com"),
                new License("License 2", "Content 2", "https://example2.com"),
                new License("License 3", "Content 3", "https://example3.com")
            };
        }

        [Test]
        public void ToLicenseDto_ShouldReturnValidDto_WhenValidLicenseProvided()
        {
            // Act
            var result = LicenseDataMapper.ToLicenseDto(_validLicense);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasValue);
            
            var licenseDto = result.Value;
            Assert.AreEqual("Test License Title", licenseDto.Title);
            Assert.AreEqual("This is a test license text content.", licenseDto.Text);
            Assert.AreEqual("https://example.com/license", licenseDto.Url);
        }

        [Test]
        public void ToLicenseDto_ShouldReturnNull_WhenLicenseIsNull()
        {
            // Act
            var result = LicenseDataMapper.ToLicenseDto(null);

            // Assert
            Assert.IsNull(result);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void ToLicenseDtoList_ShouldReturnValidDtoList_WhenValidLicenseListProvided()
        {
            // Act
            var result = LicenseDataMapper.ToLicenseDtoList(_validLicenseList);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            
            // Check first license
            Assert.AreEqual("License 1", result[0].Title);
            Assert.AreEqual("Content 1", result[0].Text);
            Assert.AreEqual("https://example1.com", result[0].Url);
            
            // Check second license
            Assert.AreEqual("License 2", result[1].Title);
            Assert.AreEqual("Content 2", result[1].Text);
            Assert.AreEqual("https://example2.com", result[1].Url);
            
            // Check third license
            Assert.AreEqual("License 3", result[2].Title);
            Assert.AreEqual("Content 3", result[2].Text);
            Assert.AreEqual("https://example3.com", result[2].Url);
        }

        [Test]
        public void ToLicenseDtoList_ShouldReturnEmptyList_WhenLicenseListIsNull()
        {
            // Act
            var result = LicenseDataMapper.ToLicenseDtoList(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ToLicenseDtoList_ShouldReturnEmptyList_WhenLicenseListIsEmpty()
        {
            // Arrange
            var emptyList = new List<License>();

            // Act
            var result = LicenseDataMapper.ToLicenseDtoList(emptyList);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ToLicenseDtoList_ShouldFilterOutNullLicenses()
        {
            // Arrange
            var listWithNulls = new List<License>
            {
                new License("Valid License", "Valid Content", "https://valid.com"),
                null,
                new License("Another Valid License", "Another Content", "https://another.com"),
                null
            };

            // Act
            var result = LicenseDataMapper.ToLicenseDtoList(listWithNulls);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Valid License", result[0].Title);
            Assert.AreEqual("Another Valid License", result[1].Title);
        }

        [Test]
        public void ToLicense_ShouldReturnValidLicense_WhenValidDtoProvided()
        {
            // Arrange
            var licenseDto = new LicenseDto(
                "Test Title",
                "Test Text",
                "https://test.com"
            );

            // Act
            var result = LicenseDataMapper.ToLicense(licenseDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Title", result.title);
            Assert.AreEqual("Test Text", result.text);
            Assert.AreEqual("https://test.com", result.url);
        }

        [Test]
        public void ToLicense_ShouldReturnNull_WhenDtoIsNull()
        {
            // Act
            var result = LicenseDataMapper.ToLicense(null);

            // Assert
            Assert.IsNull(result);
        }

        [TearDown]
        public void TearDown()
        {
            _validLicense = null;
            _validLicenseList = null;
        }
    }
}