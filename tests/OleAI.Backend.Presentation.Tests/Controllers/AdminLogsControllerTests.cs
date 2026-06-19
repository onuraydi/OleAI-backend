using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Logs.Queries.DownloadLogFile;
using OleAI.Backend.Application.Features.Logs.Queries.GetDatabaseLogs;
using OleAI.Backend.Application.Features.Logs.Queries.GetLogFiles;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers
{
    public class AdminLogsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly AdminLogsController _controller;

        public AdminLogsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new AdminLogsController();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mockMediator.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider.Object
                }
            };
        }

        [Fact]
        public async Task GetDatabaseLogs_ShouldSendQuery_AndReturnOk()
        {
            var query = new GetDatabaseLogsQuery();
            var response = new Paginate<OleAI.Backend.Application.Features.Logs.Queries.GetDatabaseLogs.SystemLogDto> { Items = new List<OleAI.Backend.Application.Features.Logs.Queries.GetDatabaseLogs.SystemLogDto>() };
            _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(response);

            var result = await _controller.GetDatabaseLogs(query);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetLogFiles_ShouldSendQuery_AndReturnOk()
        {
            var response = new List<OleAI.Backend.Application.Features.Logs.Queries.GetLogFiles.LogFileDto>
            {
                new() { Name = "log1.txt", SizeBytes = 1024, LastModified = DateTime.UtcNow }
            };
            _mockMediator.Setup(m => m.Send(It.IsAny<GetLogFilesQuery>(), default)).ReturnsAsync(response);

            var result = await _controller.GetLogFiles();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task DownloadLogFile_ShouldSendQuery_AndReturnFileResult()
        {
            var fileName = "log1.txt";
            var fileBytes = new byte[] { 1, 2, 3 };
            _mockMediator.Setup(m => m.Send(It.IsAny<DownloadLogFileQuery>(), default)).ReturnsAsync(fileBytes);

            var result = await _controller.DownloadLogFile(fileName);

            var fileResult = result as FileContentResult;
            fileResult.Should().NotBeNull();
            fileResult!.FileContents.Should().BeEquivalentTo(fileBytes);
            fileResult.ContentType.Should().Be("text/plain");
            fileResult.FileDownloadName.Should().Be(fileName);
        }
    }
}
