using Haengma.Core.Sgf;
using Xunit;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Tests.SgfAssert;
using static Xunit.Assert;

namespace Haengma.Tests.Haengma.Core.Sgf
{
    public class SgfExtensionsTest
    {
        [Fact]
        public void AddPropertyToLastNode_NoNode_NewNodeContainingProperty()
        {
            var property = new C(new("apa"));
            var tree = SgfGameTree.Empty.AddPropertyToLastNode(property);
            ContainsSingleProperty<C>(tree, x => Equal(x, property));
        }

        [Fact]
        public void AddPropertyToLastNode_ExistingNode_AppendsPropertyToExistingNode()
        {
            var firstProperty = new SZ(19);
            var secondProperty = new C(new("apa"));
            var tree = SgfGameTree.Empty.AddPropertyToLastNode(firstProperty).AddPropertyToLastNode(secondProperty);
            SingleNode(tree, node =>
            {
                Contains(firstProperty, node.Properties);
                Contains(secondProperty, node.Properties);
            });
        }

        [Fact]
        public void AddPropertyToLastNode_ExistingNode_ExistingProperty_ReplacesExistingPropertyWithNewProperty()
        {
            var firstProperty = new C(new("apa"));
            var secondProperty = new C(new("bepa"));

            var tree = SgfGameTree.Empty.AddPropertyToLastNode(firstProperty).AddPropertyToLastNode(secondProperty);
            ContainsSingleProperty<C>(tree, x => Equal(secondProperty, x));
        }
    }
}
