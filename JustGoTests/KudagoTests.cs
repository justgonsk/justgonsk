using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGoModels.Models;
using JustGoUtilities;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace JustGoTests
{
    [TestFixture]
    public class KudagoTests
    {
        private static readonly List<JObject> kudagoEvents = new List<JObject>();

        private static readonly Dictionary<JObject, JObject> eventMappings
            = new Dictionary<JObject, JObject>();

        private readonly IEnumerable<string> simpleFields = new[]
        {
            "id", "title", "short_title", "description", "body_text", "categories", "tags"
        };

        #region DatesTest

        private const string DatesTest = @"[
        {
            ""start_date"": ""2016-07-01"",
            ""start_time"": null,
            ""start"": 1467309600,
            ""end_date"": ""2016-10-02"",
            ""end_time"": null,
            ""end"": 1475427600,
            ""is_continuous"": false,
            ""is_endless"": false,
            ""is_startless"": false,
            ""schedules"": [
                {
                    ""days_of_week"": [
                        1,
                        2,
                        3,
                        4,
                        5,
                        6
                    ],
                    ""start_time"": ""11:00:00"",
                    ""end_time"": ""20:00:00""
                }
            ],
            ""use_place_schedule"": false
        },
        {
            ""start_date"": ""2019-02-12"",
            ""start_time"": ""11:00:00"",
            ""start"": 1549944000,
            ""end_date"": ""2019-05-12"",
            ""end_time"": ""21:00:00"",
            ""end"": 1557669600,
            ""is_continuous"": false,
            ""is_endless"": false,
            ""is_startless"": false,
            ""schedules"": [],
            ""use_place_schedule"": false
        },
        {
            ""start_date"": null,
            ""start_time"": null,
            ""start"": -62135433000,
            ""end_date"": ""2018-09-01"",
            ""end_time"": null,
            ""end"": 1535821200,
            ""is_continuous"": false,
            ""is_endless"": false,
            ""is_startless"": true,
            ""schedules"": [
                {
                    ""days_of_week"": [
                        1,
                        2,
                        3,
                        4,
                        5,
                        6
                    ],
                    ""start_time"": ""11:00:00"",
                    ""end_time"": ""20:00:00""
                }
            ],
            ""use_place_schedule"": false
        },
        {
            ""start_date"": null,
            ""start_time"": null,
            ""start"": -62135433000,
            ""end_date"": ""2019-01-27"",
            ""end_time"": null,
            ""end"": 1548608400,
            ""is_continuous"": false,
            ""is_endless"": false,
            ""is_startless"": true,
            ""schedules"": [
                {
                    ""days_of_week"": [
                        1,
                        2,
                        3,
                        4,
                        5,
                        6
                    ],
                    ""start_time"": ""11:00:00"",
                    ""end_time"": ""20:00:00""
                }
            ],
            ""use_place_schedule"": false
        }
    ]";

        #endregion DatesTest

        [SetUp]
        public void Setup()
        {
        }

        [TestCaseSource(nameof(Numbers), new object[] { 126000, 126020 })]
        [Order(0)]
        public async Task KudagoWorks_AndDoesntDeleteEvents(int eventId)
        {
            var parsedResponse = await Utilities
                .ParseResponseFromUrl(string.Format(Constants.EventDetailsUrlPattern, eventId));

            kudagoEvents.Add(parsedResponse);
        }

        [Order(1)]
        [Test]
        public async Task EventsParsingCausesNoExceptions()
        {
            foreach (var kudagoEvent in kudagoEvents)
            {
                var ourEvent = await KudagoConverter.ConvertEvent(kudagoEvent);
                eventMappings.Add(key: kudagoEvent, value: ourEvent);
            }
        }

        [Test]
        public void SimpleFielsParsedProperly()
        {
            foreach (var eventMapping in eventMappings)
            {
                var kudagoEvent = eventMapping.Key;
                var ourEvent = eventMapping.Value;

                foreach (var simpleField in simpleFields)
                {
                    Assert.That(kudagoEvent[simpleField], Is.EqualTo(ourEvent[simpleField]),
                        message: $"Failed on \"{simpleField}\" in event with ID = {kudagoEvent["id"]}");
                }
            }
        }

        [Test]
        public async Task SingleDatesParsedProperlyAsync()
        {
            var datesArray = JArray.Parse(DatesTest);
            var singleDates = KudagoConverter.GetSingleDates(datesArray);

            Assert.IsTrue(singleDates.Length == 1);
            Assert.AreEqual(singleDates[0].Start.ToUnixTimeSeconds(), 1549944000);
            Assert.AreEqual(singleDates[0].End.ToUnixTimeSeconds(), 1557669600);
        }

        [Test]
        public async Task ScheduledDatesParsedProperlyAsync()
        {
            var datesArray = JArray.Parse(DatesTest);
            var scheduledDates = KudagoConverter.GetScheduledDates(datesArray);

            Assert.IsTrue(scheduledDates.Length == 1);

            var onlyOneDate = scheduledDates[0];

            Assert.AreEqual(onlyOneDate.ScheduleStart.ToUnixTimeSeconds(), -62135433000);
            Assert.AreEqual(onlyOneDate.ScheduleEnd.ToUnixTimeSeconds(), 1548608400);

            Assert.IsTrue(onlyOneDate.Schedules.Count == 1);

            var onlyOneSchedule = onlyOneDate.Schedules[0];
            Assert.AreEqual(onlyOneSchedule.StartTime, TimeSpan.FromHours(11));
            Assert.AreEqual(onlyOneSchedule.EndTime, TimeSpan.FromHours(20));

            CollectionAssert.AreEquivalent(Enumerable.Range(1, 6), onlyOneSchedule.DaysOfWeek);
        }

        private static IEnumerable<int> Numbers(int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                yield return i;
            }
        }
    }
}