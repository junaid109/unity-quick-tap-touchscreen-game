using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Assets.Scripts.GoAhead;

[TestFixture]
public class TimeControllerTest {

    private GATimeController _tc;

    [SetUp]
    public void SetUp()
    {
        _tc = new GATimeController();
    }

    [Test]
    public void EditorTest() {
        //Arrange
        var gameObject = new GameObject();

        //Act
        //Try to rename the GameObject
        var newGameObjectName = "My game object";
        gameObject.name = newGameObjectName;

        //Assert
        //The object has a new name
        Assert.AreEqual(newGameObjectName, gameObject.name);
    }

    [Test]
    public void TimeElapasedTest()
    {
        
    }
}