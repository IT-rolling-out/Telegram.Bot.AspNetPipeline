using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions;
using Telegram.Bot.Types.Enums;

namespace IRO.UnitTests.Telegram
{
    public class RouteSearchBagsTests
    {
        List<RouteDescriptionData> _routeDescriptions;

        IGlobalSearchBag _sb;

        [SetUp]
        public void Setup()
        {
            _routeDescriptions = new List<RouteDescriptionData>();
            _routeDescriptions.Add(
                new RouteDescriptionData(
                    null,
                    null
                ));

            _routeDescriptions.Add(
                new RouteDescriptionData(
                    null,
                    new RouteInfo(updateTypes: UpdateTypeExtensions.All)
                ));
            _routeDescriptions.Add(
               new RouteDescriptionData(
                   null,
                   new RouteInfo(template: "        ", updateTypes: UpdateTypeExtensions.All)
               ));

            _routeDescriptions.Add(
                new RouteDescriptionData(
                    async (ctx) => { SetHandlerIdentifier(ctx, "M_LowPriority"); },
                    new RouteInfo(template: "t2", order: 2, name: "LowPriority", updateTypes: UpdateTypeExtensions.All)
                ));
            _routeDescriptions.Add(
                new RouteDescriptionData(
                     async (ctx) => { SetHandlerIdentifier(ctx, "M_HightPriority"); },
                    new RouteInfo(template: "t2", order: -1, name: "HightPriority", updateTypes: UpdateTypeExtensions.All)
                ));
            _routeDescriptions.Add(
                new RouteDescriptionData(
                     async (ctx) => { SetHandlerIdentifier(ctx, "M_NormalPriority"); },
                    new RouteInfo(template: "t2", order: 1, name: "NormalPriority", updateTypes: UpdateTypeExtensions.All)
                ));

            _routeDescriptions.Add(
               new RouteDescriptionData(
                    async (ctx) => { SetHandlerIdentifier(ctx, "Message"); },
                    new RouteInfo(template: "t3", order: 1, updateTypes: new UpdateType[] { UpdateType.Message })
               ));
            _routeDescriptions.Add(
               new RouteDescriptionData(
                    async (ctx) => { SetHandlerIdentifier(ctx, "ChannelPost"); },
                    new RouteInfo(template: "t3", order: 1, updateTypes: new UpdateType[] { UpdateType.ChannelPost })
               ));

            var provider = new GlobalSearchBagProvider();
            provider.Init(_routeDescriptions);
            _sb = provider.Resolve();

        }

        [Test]
        public void TestName()
        {
            var rdd = _sb.FindByName("LowPriority");
            var id = GetHandlerIdentifier(rdd);
            Assert.AreEqual("M_LowPriority", id);
        }

        [Test]
        public void TestNullRouteInfoRemoved()
        {
            var withoutNullRouteInfo = _sb.GetFoundData().ToList();
            Assert.AreEqual(_routeDescriptions.Count, withoutNullRouteInfo.Count + 1);
        }

        [Test]
        public void TestOrdersCount()
        {
            var list = _sb.AllSorted().ToList();
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public void TestUpdateType()
        {
            var rddMessage = RouteDescriptionData.Empty;
            foreach (var item in _sb.All())
            {
                var templateScope = item
                    .FindTemplateScope("t3");
                if (templateScope == null)
                    continue;
                var routeSearchData = templateScope.FindOneByUpdateType(UpdateType.Message);
                if (!routeSearchData.IsEmpty)
                {
                    rddMessage = routeSearchData;
                    break;
                }
            }

            var rddChannelPost = RouteDescriptionData.Empty;
            foreach (var item in _sb.All())
            {
                var templateScope = item
                    .FindTemplateScope("t3");
                if (templateScope == null)
                    continue;
                var routeSearchData = templateScope.FindOneByUpdateType(UpdateType.ChannelPost);
                if (!routeSearchData.IsEmpty)
                {
                    rddChannelPost = routeSearchData;
                    break;
                }
            }

            Assert.Multiple(() =>
            {
                Assert.AreEqual("Message", GetHandlerIdentifier(rddMessage));
                Assert.AreEqual("ChannelPost", GetHandlerIdentifier(rddChannelPost));
            });
        }

        [Test]
        public void TestNullTemplate()
        {
            var orderScopeSearchBags = _sb.All();
            int count = 0;
            foreach (var item in orderScopeSearchBags)
            {
                var templateScope = item
                    .FindTemplateScope(null);
                if (templateScope == null)
                    continue;
                count= templateScope.GetFoundData().Count();
            }
            //3 empty templates
            Assert.AreEqual(2, count);
        }

        [Test]
        public void TestDefaultSearchAndPriority()
        {
            var orderScopeSearchBags = _sb.All();
            List<RouteDescriptionData> rddList = new List<RouteDescriptionData>();
            foreach (var item in orderScopeSearchBags)
            {
                var templateScope = item
                    .FindTemplateScope("t2");
                if (templateScope == null)
                    continue;
                var routeSearchData = templateScope.FindOneByUpdateType(UpdateType.Message);
                if (!routeSearchData.IsEmpty)
                    rddList.Add(routeSearchData);
            }

            Assert.Multiple(() =>
            {
                var id1 = GetHandlerIdentifier(rddList[0]);
                Assert.AreEqual("M_HightPriority", id1);

                var id2 = GetHandlerIdentifier(rddList[1]);
                Assert.AreEqual("M_NormalPriority", id2);

                var id3 = GetHandlerIdentifier(rddList[2]);
                Assert.AreEqual("M_LowPriority", id3);
            });
        }

        #region Identify action crunch.
        readonly object _idLocker = new object();

        string _actionId;

        string GetHandlerIdentifier(RouteDescriptionData routeDescriptionData)
        {
            lock (_idLocker)
            {
                routeDescriptionData.Handler.Invoke(null);
                return _actionId;
            }
        }

        void SetHandlerIdentifier(ActionContext ctx, string id)
        {
            _actionId = id;
        }
        #endregion


    }
}
