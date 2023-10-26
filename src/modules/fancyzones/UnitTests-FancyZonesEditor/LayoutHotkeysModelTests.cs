// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FancyZonesEditor.Models;

namespace UnitTestsFancyZonesEditor;

[TestClass]
public class LayoutHotkeysModelTests
{
    public bool PropertyChangedCalled { get; set; }

    public LayoutHotkeysModel Model { get; set; } = new LayoutHotkeysModel();

    [TestInitialize]
    public void TestInitialize()
    {
        Model = new();
        Model.PropertyChanged += (object? sender, System.ComponentModel.PropertyChangedEventArgs e) =>
        {
            PropertyChangedCalled = true;
        };
    }

    [TestCleanup]
    public void TestCleanup()
    {
        PropertyChangedCalled = false;
    }

    [TestMethod]
    public void NewLayoutHotkeysModelAllHotkeysFree()
    {
        foreach (var key in Model.SelectedKeys.Keys)
        {
            Assert.AreEqual(string.Empty, Model.SelectedKeys[key]);
        }

        Assert.AreEqual(PropertyChangedCalled, false);
    }

    [TestMethod]
    public void LayoutShouldOnlyBeAssignedOnce()
    {
        var initialHotKey = "1";
        var layoutId = "layout1";

        Model.SelectKey(initialHotKey, layoutId);
        Model.SelectKey("2", layoutId);

        Assert.AreEqual(string.Empty, Model.SelectedKeys[initialHotKey]);
        Assert.AreEqual(PropertyChangedCalled, true);
    }

    [TestMethod]
    public void NoneStringReturnedIfLayoutNotSetToHotkey()
    {
        var noneOption = "None"; // Will this break if the localization in a test is different?

        Assert.AreEqual(noneOption, Model.Key("bogus"));
    }

    [TestMethod]
    public void FreeKeyShouldSetLayoutToNone()
    {
        var initialHotKey = "1";
        var layoutId = "layout1";

        Model.SelectKey(initialHotKey, layoutId);
        Model.FreeKey(initialHotKey);

        Assert.AreEqual(string.Empty, Model.SelectedKeys[initialHotKey]);
        Assert.AreEqual(PropertyChangedCalled, true);
    }

    [TestMethod]
    public void CleanUpFreesAllHotkeys()
    {
        Model.SelectKey("1", "layout1");
        Model.SelectKey("2", "layout2");
        Model.SelectKey("3", "layout3");

        Model.CleanUp();

        foreach (var key in Model.SelectedKeys.Keys)
        {
            Assert.AreEqual(string.Empty, Model.SelectedKeys[key]);
        }

        Assert.AreEqual(PropertyChangedCalled, true);
    }
}
