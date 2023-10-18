// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FancyZonesEditor.Models;

namespace UnitTestsFancyZonesEditor;

[TestClass]
public class GridLayoutModelTests
{
    [TestMethod]
    public void EmptyGridLayoutModelIsNotValid()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        Assert.IsFalse(gridLayoutModel.IsModelValid());
    }

    [TestMethod]
    public void GridLayoutModelWithInvalidRowAndColumnCountsIsNotValid()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.Rows = 0;
        gridLayoutModel.Columns = 0;
        Assert.IsFalse(gridLayoutModel.IsModelValid());
    }

    [TestMethod]
    public void GridLayoutModelWithInvalidRowPercentsIsNotValid()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.Rows = 1;
        gridLayoutModel.Columns = 1;
        gridLayoutModel.RowPercents = new List<int> { 0 }; // Invalid percentage
        Assert.IsFalse(gridLayoutModel.IsModelValid());
    }

    [TestMethod]
    public void GridLayoutModelWithInvalidColumnPercentsIsNotValid()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.Rows = 1;
        gridLayoutModel.Columns = 1;
        gridLayoutModel.ColumnPercents = new List<int> { 0 }; // Invalid percentage
        Assert.IsFalse(gridLayoutModel.IsModelValid());
    }

    [TestMethod]
    public void GridLayoutModelWithInvalidCellChildMapLengthIsNotValid()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.Rows = 2;
        gridLayoutModel.Columns = 2;
        gridLayoutModel.CellChildMap = new int[2, 1]; // Invalid length
        Assert.IsFalse(gridLayoutModel.IsModelValid());
    }

    [TestMethod]
    public void GridLayoutModelWithInvalidZoneCountIsNotValid()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.Rows = 2;
        gridLayoutModel.Columns = 2;
        gridLayoutModel.CellChildMap = new int[,]
        {
            { 1, 2 },
            { 3, 4 },
        }; // Invalid zone count
        Assert.IsFalse(gridLayoutModel.IsModelValid());
    }

    [TestMethod]
    public void GridLayoutModelWithValidPropertiesIsValid()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();

        // Set valid row and column counts
        gridLayoutModel.Rows = 2;
        gridLayoutModel.Columns = 2;

        // Set valid percentages for rows and columns
        // Should add up to 10000
        gridLayoutModel.RowPercents = new List<int> { 5000, 5000 };
        gridLayoutModel.ColumnPercents = new List<int> { 5000, 5000 };

        // Set a valid CellChildMap
        gridLayoutModel.CellChildMap = new int[,]
        {
            { 0, 1 },
            { 2, 3 },
        }; // corresponds to 4 zones

        Assert.IsTrue(gridLayoutModel.IsModelValid(), "GridLayoutModel with valid properties should be valid.");
    }

    [TestMethod]
    public void InitColumnsShouldSetValidColumnPercents()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.TemplateZoneCount = 2;
        gridLayoutModel.Type = LayoutType.Columns;

        gridLayoutModel.InitTemplateZones();

        Assert.AreEqual(2, gridLayoutModel.ColumnPercents.Count);
        Assert.AreEqual(5000, gridLayoutModel.ColumnPercents[0]);
        Assert.AreEqual(5000, gridLayoutModel.ColumnPercents[1]);
    }

    [TestMethod]
    public void InitRowsShouldSetValidRowPercents()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.TemplateZoneCount = 3;
        gridLayoutModel.Type = LayoutType.Rows;

        gridLayoutModel.InitTemplateZones();

        Assert.AreEqual(3, gridLayoutModel.RowPercents.Count);
        Assert.AreEqual(3333, gridLayoutModel.RowPercents[0]);
        Assert.AreEqual(3333, gridLayoutModel.RowPercents[1]);
        Assert.AreEqual(3334, gridLayoutModel.RowPercents[2]); // Last one gets left over percentage
    }

    [TestMethod]
    public void InitGridPerfectlyDivisibleMakesPerfectGrid()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.TemplateZoneCount = 4;
        gridLayoutModel.Type = LayoutType.Grid;

        gridLayoutModel.InitTemplateZones();

        Assert.AreEqual(2, gridLayoutModel.Rows);
        Assert.AreEqual(2, gridLayoutModel.Columns);

        // Perfect Grid means no cells share same 'id'
        var ids = gridLayoutModel.CellChildMap.Cast<int>();
        Assert.AreEqual(gridLayoutModel.TemplateZoneCount, ids.Count());
    }

    [TestMethod]
    public void InitGridNotPerfectlyDivisibleLastCellsGetCombined()
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.TemplateZoneCount = 5;
        gridLayoutModel.Type = LayoutType.Grid;

        gridLayoutModel.InitTemplateZones();

        Assert.AreEqual(2, gridLayoutModel.Rows);
        Assert.AreEqual(3, gridLayoutModel.Columns);

        // Last cells share same 'id' so they get combined
        Assert.AreEqual(4, gridLayoutModel.CellChildMap[1, 1]);
        Assert.AreEqual(4, gridLayoutModel.CellChildMap[1, 2]);
    }

    [TestMethod]
    [DynamicData(nameof(GetGridLayoutModelTestData), DynamicDataSourceType.Method)]
    public void GridLayoutModelIsValid(int rows, int columns, bool expected)
    {
        GridLayoutModel gridLayoutModel = new GridLayoutModel();
        gridLayoutModel.Rows = rows;
        gridLayoutModel.Columns = columns;
        Assert.AreEqual(expected, gridLayoutModel.IsModelValid());
    }

    public static IEnumerable<object[]> GetGridLayoutModelTestData()
    {
        yield return new object[] { 0, 0, false };
        yield return new object[] { 1, 1, false };
        yield return new object[] { 2, 1, false };
        yield return new object[] { 1, 2, false };
        yield return new object[] { 2, 2, true };
    }
}
