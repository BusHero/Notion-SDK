﻿using FluentAssertions;

using Microsoft.Extensions.Configuration;

using Xunit;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using System.Linq;
using Notion.Model;
using System.Collections.Generic;

namespace Notion.Sharp.Tests
{
    public class ModelTests
    {
        #region Ids
        private INotion SUT { get; }

        private Guid ValidUserId { get; }

        private Guid ValidDatabaseId { get; }

        private Guid ValidPageId { get; }

        private Guid ValidBlockId { get; }

        private Guid PageFromDatabase { get; }

        #endregion

        public ModelTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<ModelTests>()
                .Build();

            SUT = Notion.NewClient(bearerToken: configuration["Notion"], "2021-08-16");
            ValidUserId = Guid.Parse(configuration["userId"]);
            ValidDatabaseId = Guid.Parse(configuration["databaseId"]);
            ValidPageId = Guid.Parse(configuration["pageId"]);
            ValidBlockId = Guid.Parse(configuration["blockId"]);
            PageFromDatabase = Guid.Parse(configuration["pageFromDatabase"]);
        }

        #region Users

        [Fact]
        public async Task GetUsers()
        {
            PaginationList<User> users = await SUT.GetUsersAsync();
            users.Should().NotBeNull();
        }

        [Fact]
        public async Task GetMe()
        {
            User me = await SUT.GetMeAsync();
            me.Should().BeOfType<User.Bot>();
        }

        [Fact]
        public async Task GetUser_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.GetUserAsync(Guid.NewGuid()))
                .Should()
                .ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task GetUser_Succeds_OnValidId()
        {
            User user = await SUT.GetUserAsync(ValidUserId);
            user.Should().NotBeNull();
        }

        #endregion

        #region Databases

        [Fact]
        public async Task UpdateDatabase_Succeds()
        {
            await SUT.UpdateDatabaseAsync(ValidDatabaseId, new
            {
                title = new object[]
                {
                    new
                    {
                        text = new
                        {
                            content = "some new title"
                        }
                    }
                }
            });
        }

        [Fact]
        public async Task CreateDatabase_Succeds()
        {
            await SUT.CreateDatabaseAsync(new
            {
                parent = new
                {
                    page_id = ValidPageId
                },
                title = new object[]
                {
                    new
                    {
                        text = new
                        {
                            content = "some new title"
                        }
                    }
                },
                properties = new
                {
                    Name = new
                    {
                        title = new object()
                    }
                }
            });
        }

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
            Database database = await SUT.GetDatabaseAsync(ValidDatabaseId);
            database.Should().NotBeNull();
        }

        [Fact]
        public async Task QueryDatabase_Succeds()
        {
            PaginationList<Page> results = await SUT.QueryDatabaseAsync(ValidDatabaseId, new
            {

            });
            results.Should().NotBeNull();
        }

        [Fact]
        public async Task QueryDatabase_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.QueryDatabaseAsync(Guid.NewGuid(), new
            {

            })).Should().ThrowAsync<NotionException>();
        }

        #endregion

        #region Pages

        [Fact]
        public async Task UpdatePage_Succeds_OnValidId()
        {
            await SUT.UpdatePageAsync(ValidPageId, new
            {
                properties = new
                {
                    title = new
                    {
                        title = new object[]
                        {
                            new
                            {
                                text = new
                                {
                                    content = "some new title"
                                }
                            }
                        }
                    }
                }
            });
        }

        [Fact]
        public async Task UpdatePage_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.UpdatePageAsync(Guid.NewGuid(), new
            {
                properties = new
                {
                    title = new
                    {
                        title = new object[]
                        {
                            new
                            {
                                text = new
                                {
                                    content = "some new title"
                                }
                            }
                        }
                    }
                }
            })).Should().ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task GetPage_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.GetPageAsync(Guid.NewGuid())).Should().ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task GetPage_Succeds_OnValidId()
        {
            Page page = await SUT.GetPageAsync(ValidPageId);
            page.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPageFromDatabase_Succeds()
        {
            Page page = await SUT.GetPageAsync(PageFromDatabase);
            page.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePage_Succeds()
        {
            var result = await SUT.CreatePageAsync(new Page
            {
                Parent = new Parent.Page
                {
                    Id = ValidPageId
                },
                Properties = new Dictionary<string, PropertyValue>
                {
                    ["title"] = new PropertyValue.Title
                    {
                        Content = new RichText[]
                        {
                            new RichText.Text
                            {
                                Content = "Some content here and there"
                            }
                        }
                    }
                }
            });
            result.Should().NotBeNull();
            //await SUT.CreatePageAsync(new
            //{
            //    parent = new
            //    {
            //        page_id = ValidPageId
            //    },
            //    properties = new
            //    {
            //        title = new
            //        {
            //            title = new object[]
            //            {
            //                new
            //                {
            //                    text = new
            //                    {
            //                        content = "some new title"
            //                    }
            //                }
            //            }
            //        }
            //    }
            //});
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
            var blocks = await SUT.GetBlocksChildrenAsync(ValidPageId);
            blocks.Should().NotBeNull();
        }

        [Fact]
        public async Task GetBlock_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.GetBlockAsync(Guid.NewGuid())).Should().ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task GetBlock_Succeds_OnValidId()
        {
            var block = await SUT.GetBlockAsync(ValidBlockId);
            block.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(Blocks))]
        public async Task AppendChildren_Succeds(Block block)
        {
            var result = await SUT.AppendBlockChildrenAsync(ValidPageId,
               new List<Block>
               {
                   block
               }

                //    new
                //{
                //    children = new object[]
                //    {
                //        new
                //        {
                //            heading_2 = new
                //            {
                //                text = new object[]
                //                {
                //                    new
                //                    {
                //                        text = new
                //                        {
                //                            content = "Brave new world!"
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                );
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task AppendChildren_Fails_OnInvalidId()
        {
            var result = await SUT.Awaiting(sut => sut.AppendBlockChildrenAsync(Guid.NewGuid(), new List<Block>
               {
                   new Block.Heading2
                   {
                       Text = new RichText[]
                       {
                           new RichText.Text
                           {
                               Content = "Brave new world"
                           }
                       }
                   }
               })).Should().ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task UpdateBlock_Succeds()
        {
            var result = await SUT.UpdateBlockAsync(ValidBlockId, new
            {
                paragraph = new
                {
                    text = new object[]
                    {
                        new
                        {
                            text = new
                            {
                                content = "hello to you"
                            }
                        }
                    }
                }
            });
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateBlock_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.UpdateBlockAsync(Guid.NewGuid(), new
            {
                paragraph = new
                {
                    text = new object[]
                    {
                        new
                        {
                            text = new
                            {
                                content = "hello to you"
                            }
                        }
                    }
                }
            })).Should().ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task DeleteBlock_Fails_OnInvalidId()
        {
            await SUT.Awaiting(sut => sut.DeleteBlockAsync(Guid.NewGuid()))
                .Should()
                .ThrowAsync<NotionException>();
        }

        [Fact]
        public async Task DeleteBlock_Succeds()
        {
            var result = await SUT.AppendBlockChildrenAsync(ValidPageId, new List<Block>
               {
                   new Block.Heading2
                   {
                       Text = new RichText[]
                       {
                           new RichText.Text
                           {
                               Content = "Brave new world"
                           }
                       }
                   }
               });
            var result2 = await SUT.DeleteBlockAsync(result.Results[0].Id);
            result2.Should().NotBeNull();
        }

        #endregion

        [Fact]
        public async Task Search_Succeds_OnValidParameter()
        {
            //var result = await SUT.SearchAsync("foo");
            PaginationList<PageOrDatabase> result = await SUT.SearchAsync(
                new(
                //query: "foo",
                //sort: new Sort(direction: "ascending", timestamp: "last_edited_time"),
                //filter: new Filter(value: "database", property: "object"),
                //page_size: 100
                )
                );
            result.Should().NotBeNull();
        }

        public static TheoryData<Block> Blocks { get; } = new TheoryData<Block>
        {
            new Block.Heading1
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Heading 1"
                    }
                }
            },
            new Block.Heading2
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Heading 2"
                    }
                }
            },
            new Block.Heading3
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Heading 3"
                    }
                }
            },
            new Block.Paragraph
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Paragraph"
                    }
                },
                Children = new Block[]
                {
                    new Block.Paragraph
                    {
                        Text = new RichText[]
                        {
                            new RichText.Text
                            {
                                Content = "Child paragraph"
                            }
                        }
                    }
                }
            },
            new Block.BulletedListItem
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Bulleted list item"
                    }
                },
                Children = new Block[]
                {
                    new Block.BulletedListItem
                    {
                        Text = new RichText[]
                        {
                            new RichText.Text
                            {
                                Content = "Child content"
                            }
                        }
                    }
                }
            },
            new Block.NumberedListItem
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Numbered list item"
                    }
                },
                Children = new Block[]
                {
                    new Block.BulletedListItem
                    {
                        Text = new RichText[]
                        {
                            new RichText.Text
                            {
                                Content = "Child content"
                            }
                        }
                    }
                }
            },
            new Block.ToDo
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "to do"
                    }
                },
                Children = new Block[]
                {
                    new Block.BulletedListItem
                    {
                        Text = new RichText[]
                        {
                            new RichText.Text
                            {
                                Content = "Child content"
                            }
                        }
                    }
                }
            },
            new Block.Toggle
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "toggle"
                    }
                },
                Children = new Block[]
                {
                    new Block.BulletedListItem
                    {
                        Text = new RichText[]
                        {
                            new RichText.Text
                            {
                                Content = "Child content"
                            }
                        }
                    }
                },
            },
            new Block.Code
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "var x = 1 + 1;"
                    }
                },
                Language = "c#"
            },
            new Block.Bookmark
            {
                Caption = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Caption text"
                    }
                },
                Url = new Uri("https://google.com")
            },
            new Block.Quote
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Quote text"
                    }
                }
            },
            new Block.Callout
            {
                Text = new RichText[]
                {
                    new RichText.Text
                    {
                        Content = "Callout"
                    }
                }
            },
            new Block.Divider
            {
            },
            new Block.TableOfContents
            { 
            },
            new Block.Equation
            {
                Expression = "1 + 1"
            }
        };
    }
}
