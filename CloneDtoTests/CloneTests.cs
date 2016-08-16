using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CloneDto;

namespace CloneDtoTests
{
    [TestClass]
    public class CloneTests
    {
        [TestMethod]
        public void TestICloneableClone()
        {
            var dto1 = new CloneableDto { Id = 100, Name = "Abc", BirthDate = new DateTime(1964, 12, 31) };
            var dto2 = dto1.Clone();
            IsValidClone(dto1, dto2);
        }

        [TestMethod]
        public void TestMemberwiseClone()
        {
            var dto1 = CreateBasicDto();
            var dto2 = CloneUtility.MemberwiseClone(dto1);
            IsValidClone(dto1, dto2);
        }

        [TestMethod]
        public void TestSerializeClone1()
        {
            var dto1 = CreateSimpleDto();
            var dto2 = CloneUtility.SerializableClone(dto1);
            IsValidClone(dto1, dto2);
        }

        [TestMethod]
        public void TestSerializeClone2()
        {
            var dto1 = CreateComplexDto();
            var dto2 = CloneUtility.SerializableClone(dto1);
            IsValidClone(dto1, dto2);
        }

        [TestMethod]
        public void TestXmlSerializer1()
        {
            var dto1 = CreateBasicDto();
            var dto2 = CloneUtility.XmlSerializerClone(dto1);
            IsValidClone(dto1, dto2);
        }

        [TestMethod]
        public void TestXmlSerializer2()
        {
            var dto1 = CreateComplexDto();
            var dto2 = CloneUtility.XmlSerializerClone(dto1);
            IsValidClone(dto1, dto2);
        }

        [TestMethod]
        public void TestExpressionTree()
        {
            // Could store compiled expression tree for reuse, possibly in cached dictionary with key being type.
            var dlg = CloneUtility.CreateCloneDelegate(typeof(BasicDto));
            var dto1 = CreateBasicDto();
            var dto2 = dlg(dto1);
            IsValidClone(dto1, dto2);
        }

        [TestMethod]
        public void TestReflectionClone()
        {
            var dto1 = CreateSimpleDto();
            var dto2 = CloneUtility.ReflectionDeepClone(dto1);
            IsValidClone(dto1, dto2);
        }
        #region Helper Methods

        private static BasicDto CreateBasicDto()
        {
            return new BasicDto { Id = 100, Name = "Abc", BirthDate = new DateTime(1964, 12, 31) };
        }

        private static SimpleDto CreateSimpleDto()
        {
            return new SimpleDto { Id = 100, Name = "Abc", BirthDate = new DateTime(1964, 12, 31) };
        }

        private static ComplexDto CreateComplexDto()
        {
            return new ComplexDto
            {
                Id = 100,
                Name = "Abc",
                BirthDate = new DateTime(1964, 12, 31),
                Details = new List<Detail>
                {
                    new Detail { Name = "A" },
                    new Detail { Name = "B" },
                    new Detail { Name = "C" }
                }
            };
        }

        private static void IsValidClone(object dto1, object dto2)
        {
            if (dto1 == dto2)
            {
                Assert.Fail("Same object references");
            }

            var dtoString1 = TestHelper.SerializeObject(dto1);
            var dtoString2 = TestHelper.SerializeObject(dto2);
            Assert.AreEqual(dtoString1, dtoString2);
        }
        #endregion
    }
}