using Haengma.SGF;
using Haengma.SGF.ValueTypes;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Haengma.GS
{
    public class GameHub : Hub
    {
        private SgfGameTree _gameTree;

        public void NewGame()
        {
            _gameTree = new SgfGameTree();
        }

        public void AddBlackMove(int x, int y)
        {
            var node = new SgfNode
            {
                Properties =
                {
                    new SgfProperty("B")
                    {
                        Values =
                        {
                            new SgfPoint(x, y)
                        }
                    }
                }
            };
        }

        public void AddWhiteMove(int x, int y)
        {

        }
    }
}
