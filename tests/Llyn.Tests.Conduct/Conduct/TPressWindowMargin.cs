using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TPressWindowMargin
{
    [Fact]
    public async Task TopSet_TypedText_ChangesTopOnly()
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            press.TPressMarginSet("1", "2", "3", "4");
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressTopSet("5");

            Assert.Equal(
                CSPressMargin.CSPressMarginCustom,
                setting.CSPressSettingMargin.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Equal(5.0, setting.CSPressSettingTop!.Value, 6);
            Assert.Equal(2.0, setting.CSPressSettingBottom!.Value, 6);
            Assert.Equal(3.0, setting.CSPressSettingLeft!.Value, 6);
            Assert.Equal(4.0, setting.CSPressSettingRight!.Value, 6);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Fact]
    public async Task BottomSet_TypedText_ChangesBottomOnly()
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            press.TPressMarginSet("1", "2", "3", "4");
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressBottomSet("5");

            Assert.Equal(
                CSPressMargin.CSPressMarginCustom,
                setting.CSPressSettingMargin.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Equal(1.0, setting.CSPressSettingTop!.Value, 6);
            Assert.Equal(5.0, setting.CSPressSettingBottom!.Value, 6);
            Assert.Equal(3.0, setting.CSPressSettingLeft!.Value, 6);
            Assert.Equal(4.0, setting.CSPressSettingRight!.Value, 6);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Fact]
    public async Task LeftSet_TypedText_ChangesLeftOnly()
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            press.TPressMarginSet("1", "2", "3", "4");
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressLeftSet("2");

            Assert.Equal(
                CSPressMargin.CSPressMarginCustom,
                setting.CSPressSettingMargin.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Equal(1.0, setting.CSPressSettingTop!.Value, 6);
            Assert.Equal(2.0, setting.CSPressSettingBottom!.Value, 6);
            Assert.Equal(2.0, setting.CSPressSettingLeft!.Value, 6);
            Assert.Equal(4.0, setting.CSPressSettingRight!.Value, 6);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }

    [Fact]
    public async Task RightSet_TypedText_ChangesRightOnly()
    {
        List<CSPressSetting> raised = [];
        await TInterfaceConductPortrait.TPressWindowOpen(new TPressBoard(), press =>
        {
            press.TPressMarginSet("1", "2", "3", "4");
            press.TPressSettingAttach(raised.Add);

            CSPressSetting setting = press.TPressRightSet("2");

            Assert.Equal(
                CSPressMargin.CSPressMarginCustom,
                setting.CSPressSettingMargin.Single(static choice => choice.CSPressChoiceChosen).CSPressChoiceValue);
            Assert.Equal(1.0, setting.CSPressSettingTop!.Value, 6);
            Assert.Equal(2.0, setting.CSPressSettingBottom!.Value, 6);
            Assert.Equal(3.0, setting.CSPressSettingLeft!.Value, 6);
            Assert.Equal(2.0, setting.CSPressSettingRight!.Value, 6);
            return Task.CompletedTask;
        });

        Assert.Single(raised);
    }
}
