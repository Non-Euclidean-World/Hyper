using FluentAssertions;
using Hyper.MarchingCubes;
using OpenTK.Mathematics;

namespace HyperTest.MarchingCubesTests
{
    [TestFixture]
    public class GeneratorTests
    {
        private Generator _generator;

        [SetUp]
        public void SetUp()
        {
            _generator = new Generator();
        }

        [Test]
        public void TestGetNormal()
        {
            // Arrange
            var triangle = new Triangle
            {
                A = new Vector3(1, 0, 0),
                B = new Vector3(0, 1, 0),
                C = new Vector3(0, 0, 1)
            };
            var expectedNormal = Vector3.Cross(triangle.B - triangle.A, triangle.C - triangle.A).Normalized();

            // Act
            var result = _generator.GetNormal(triangle);

            // Assert
            result.Should().Be(expectedNormal);
        }

        [Test]
        public void TestGetNormals()
        {
            // Arrange
            var triangles = new Triangle[]
            {
            new Triangle
            {
                A = new Vector3(1, 0, 0),
                B = new Vector3(0, 1, 0),
                C = new Vector3(0, 0, 1)
            },
            new Triangle
            {
                A = new Vector3(0, 0, 0),
                B = new Vector3(1, 0, 0),
                C = new Vector3(0, 1, 0)
            }
            };
            var expectedNormals = triangles.Select(t => Vector3.Cross(t.B - t.A, t.C - t.A).Normalized()).ToArray();

            // Act
            var result = _generator.GetNormals(triangles);

            // Assert
            CollectionAssert.AreEqual(expectedNormals, result);
            result.Should().BeEquivalentTo(expectedNormals);
        }

        [Test]
        public void TestGetTriangleAndNormalData()
        {
            // Arrange
            var triangles = new Triangle[]
            {
            new Triangle
            {
                A = new Vector3(1, 0, 0),
                B = new Vector3(0, 1, 0),
                C = new Vector3(0, 0, 1)
            },
            new Triangle
            {
                A = new Vector3(0, 0, 0),
                B = new Vector3(1, 0, 0),
                C = new Vector3(0, 1, 0)
            }
            };
            var normals = triangles.Select(t => Vector3.Cross(t.B - t.A, t.C - t.A).Normalized()).ToArray();
            var expectedData = triangles.SelectMany((t, i) => new float[]
            {
                t.A.X, t.A.Y, t.A.Z, normals[i].X, normals[i].Y, normals[i].Z,
                t.B.X, t.B.Y, t.B.Z, normals[i].X, normals[i].Y, normals[i].Z,
                t.C.X, t.C.Y, t.C.Z, normals[i].X, normals[i].Y, normals[i].Z
            }).ToArray();

            // Act
            var result = _generator.GetTriangleAndNormalData(triangles, normals);

            // Assert
            result.Should().BeEquivalentTo(expectedData);
        }

        [Test]
        public void TestGetTriangleAndNormalData_Exception()
        {
            // Arrange
            var triangles = new Triangle[]
            {
                new Triangle
                {
                    A = new Vector3(1, 0, 0),
                    B = new Vector3(0, 1, 0),
                    C = new Vector3(0, 0, 1)
                }
            };
            var normals = new Vector3[]
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 0)
            };

            // Act & Assert
            _generator.Invoking(x => x.GetTriangleAndNormalData(triangles, normals)).Should().Throw<ArgumentException>();
        }
    }
}
