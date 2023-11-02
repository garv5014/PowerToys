// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using FancyZonesEditor.Models;

namespace UnitTestFancyZonesEditor;

[TestClass]
public class CanvasLayoutModelTests
{
    public CanvasLayoutModel Model { get; set; } = new CanvasLayoutModel("layout");

    private List<string> propertiesChanged = new();

    [TestInitialize]
    public void TestInitialize()
    {
        Model = new("layout");
        Model.PropertyChanged += (object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
        {
            propertiesChanged.Add(e.PropertyName ?? string.Empty);
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        propertiesChanged.Clear();
    }

    [TestMethod]
    public void AddingOneZoneToCanvasLayoutModel()
    {
        // add a zone
        var zone1 = new Int32Rect(0, 0, 100, 100);
        var zone2 = new Int32Rect(0, 0, 100, 100);
        Model.AddZone(zone1);
        List<Int32Rect> expectedZones = new List<Int32Rect>() { zone2 };

        // assert zone is added
        CollectionAssert.AreEquivalent(expectedZones, Model.Zones.ToList());
        CollectionAssert.AreEquivalent(propertiesChanged, new List<string>() { "TemplateZoneCount", "IsZoneAddingAllowed", "UpdateLayout" });
    }
}
