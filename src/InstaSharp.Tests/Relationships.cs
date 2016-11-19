﻿using System;
using System.Net;
using System.Threading.Tasks;
using InstaSharp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InstaSharp.Tests
{
    [TestClass]
    public class Relationships : TestBase
    {
        readonly Endpoints.Relationships relationships;

        public Relationships()
        {
            relationships = new Endpoints.Relationships(Config, Auth);
        }

        [TestMethod, TestCategory("Relationships.Follows")]
        public async Task Follows()
        {
            var result = await relationships.Follows();
            Assert.IsTrue(result.Meta.Code == HttpStatusCode.OK);
        }

        [TestMethod, TestCategory("Relationships.Follows")]
        public async Task Follows_Id()
        {
            var result = await relationships.Follows(Auth.User.Id);
            Assert.IsTrue(result.Data.Count > 0);
        }

        [TestMethod, TestCategory("Relationships.Follows")]
        public async Task Follows_NextCursor()
        {
            //This test will fail if testing with an account with less than one page of follows
            var result = await relationships.Follows();
            result = await relationships.Follows(524549267/*microsoft*/, result.Pagination.NextCursor);
            Assert.IsTrue(result.Data.Count > 0);
        }

        [TestMethod, TestCategory("Relationships.Follows")]
        public async Task FollowsAll()
        {
            var result = await relationships.FollowsAll(524549267);/*microsoft*/
            Assert.IsTrue(result.Count > 50);
        }
        [TestMethod, TestCategory("Relationships.FollowedBy")]
        public async Task FollowedBy()
        {
            var result = await relationships.FollowedBy();
            Assert.IsTrue(result.Data.Count > 0);
        }

        [TestMethod, TestCategory("Relationships.FollowedBy")]
        public async Task FollowedBy_Id()
        {
            var result = await relationships.FollowedBy(Auth.User.Id);
            Assert.IsTrue(result.Data.Count > 0);
        }

        [TestMethod, TestCategory("Relationships.FollowedBy")]
        public async Task FollowedByAll()
        {
            var result = await relationships.FollowedByAll();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod, TestCategory("Relationships.FollowedBy")]
        public async Task FollowedBy_NextCursor()
        {
            //This test will fail if testing with an account with less than one page of followers
            var result = await relationships.FollowedBy();
            result = await relationships.FollowedBy(Auth.User.Id, result.Pagination.NextCursor);
            Assert.IsTrue(result.Data.Count > 0);
        }

        [TestMethod, TestCategory("Relationships.RequestedBy")]
        public async Task RequestedBy()
        {
            var result = await relationships.RequestedBy();

            Assert.IsTrue(result.Meta.Code == HttpStatusCode.OK);
        }

        [TestMethod, TestCategory("Relationships.RelationshipNone")]
        public async Task RelationshipNone()
        {
            var follow = await relationships.Relationship(3);
            Assert.AreEqual(follow.Data.OutgoingStatus, OutgoingStatus.None);
            Assert.AreEqual(follow.Data.IncomingStatus, IncomingStatus.None);
        }

        [TestMethod, TestCategory("Relationships.RelationshipOutgoingStatusFollowedBy")]
        public async Task RelationshipOutgoingStatusFollowedBy()
        {
            var follow = await relationships.Relationship(524549267);
            Assert.AreEqual(follow.Data.OutgoingStatus, OutgoingStatus.Follows);
            Assert.AreEqual(follow.Data.IncomingStatus, IncomingStatus.None);
        }

        [TestMethod, TestCategory("Relationships.RelationshipIncomingStatusFollowedBy")]
        public async Task RelationshipIncomingStatusFollowedBy()
        {
            var follow = await relationships.Relationship(457273003);
            Assert.AreEqual(follow.Data.OutgoingStatus, OutgoingStatus.None);
            Assert.AreEqual(follow.Data.IncomingStatus, IncomingStatus.FollowedBy);
        }

        [TestMethod, TestCategory("Relationships.Relationship")]
        public async Task RelationshipAction()
        {
            var follow = await relationships.Relationship(3, Endpoints.Relationships.Action.Follow);
            Assert.IsTrue(follow.Data.OutgoingStatus == OutgoingStatus.Follows, "Failed on follow");

            var unfollow = await relationships.Relationship(3, Endpoints.Relationships.Action.Unfollow);
            Assert.IsTrue(unfollow.Data.OutgoingStatus == OutgoingStatus.None, "Failed on unfollow");
        }

        [TestMethod, TestCategory("Relationships.RelationshipBlock")]
        public async Task RelationshipBlock()
        {
            var blockStatus = await relationships.Relationship(3, Endpoints.Relationships.Action.Block);
            Assert.IsTrue(blockStatus.Data.IncomingStatus == IncomingStatus.BlockedbyYou, "Failed on block");

            var unBlockStatus = await relationships.Relationship(3, Endpoints.Relationships.Action.Unblock);
            Assert.IsTrue(unBlockStatus.Data.IncomingStatus != IncomingStatus.BlockedbyYou, "Failed on unblock");
        }
    }
}
