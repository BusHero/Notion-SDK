﻿using FluentAssertions;

using Microsoft.Extensions.Configuration;

using Xunit;
using System.Threading.Tasks;
using System;

namespace Notion.Sdk.Tests
{
    public class ModelTests
    {
        private INotion SUT { get; }
        private Guid ValidUserId { get; }
        private Guid ValidDatabaseId {  get; }
        private Guid ValidPageId { get; }

        public ModelTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<ModelTests>()
                .Build();

            SUT = Notion.NewClient(bearerToken: configuration["Notion"]);
            ValidUserId = Guid.Parse(configuration["userId"]);
            ValidDatabaseId = Guid.Parse(configuration["databaseId"]);
            ValidPageId = Guid.Parse(configuration["pageId"]);
        }

        #region Users

        [Fact]
        public async Task GetUsers()
        {
            var users = await SUT.GetUsersAsync();
            users.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetMe()
        {
            var me = await SUT.GetMeAsync();
            me.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetUser_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.GetUserAsync(System.Guid.NewGuid()))
                .Should()
                .ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task GetUser_Succeds_OnValidId()
        {
            var user = await SUT.GetUserAsync(ValidUserId);
            user.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region Databases

        [Fact]
        public async Task GetDatabase_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.GetDatabaseAsync(Guid.NewGuid()))
                .Should()
                .ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task GetDatabase_Succeds_OnValidId()
        {
            var database = await SUT.GetDatabaseAsync(ValidDatabaseId);
            database.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region Pages

        [Fact]
        public async Task GetPage_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.GetPageAsync(Guid.NewGuid())).Should().ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task GetPage_Succeds_OnValidId()
        {
            var user = await SUT.GetPageAsync(ValidPageId);
            user.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region Blocks

        [Fact]
        public async Task GetBlocksChildren_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.GetBlocksChildrenAsync(Guid.NewGuid())).Should().ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task GetBlocks_Succeds_OnValidId()
        {
            var block = await SUT.GetBlocksChildrenAsync(ValidPageId);
            block.Should().NotBeNullOrEmpty(); 
        }

        #endregion

        [Fact]
        public async Task Search_Succeds_OnValidParameter()
        {
            string result = await SUT.SearchAsync(new(
                query: "foo",
                sort: new Sort(direction: "ascending", timestamp: "last_edited_time"),
                filter: new Filter(value: "database", property: "object"),
                page_size: 100));
            result.Should().NotBeNullOrEmpty();
        }
    }
}
