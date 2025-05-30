using Xunit;
using Moq;
using BlockedCountriesAPI.Controllers;
using BlockedCountriesAPI.Repositories;
using BlockedCountriesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;
using System;
using BlockedCountriesAPI.Repositories.IRepositories;
using BlockedCountriesAPI.Services;
using BlockedCountriesAPI.Services.IServices;
using BlockedCountriesAPI.Models.DTOs;

public class IpControllerTests
{
    [Fact]
    public async Task CheckBlock_ReturnsBlockedStatus_WhenCountryIsBlocked()
    {
        // Arrange
        var ip = "8.8.8.8";
        var ipInfo = new IpInfo { CountryCode = "US", Country = "United States" };





        var mockIpLookupService = new Mock<IIpLookupService>(); 
        mockIpLookupService.Setup(s => s.GetIpInfoAsync(ip)).ReturnsAsync(ipInfo);

        var mockBlockedRepo = new Mock<IBlockedCountryRepository>();
        mockBlockedRepo.Setup(r => r.Exists("US")).Returns(true);

        var mockAttemptsRepo = new Mock<IBlockedAttemptsRepository>();
        mockAttemptsRepo.Setup(r => r.AddAttempt(It.IsAny<BlockedAttempt>()));

        var controller = new IpController(mockIpLookupService.Object, mockAttemptsRepo.Object, mockBlockedRepo.Object);

        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(ip);
        context.Request.Headers["User-Agent"] = "UnitTestAgent";
        controller.ControllerContext = new ControllerContext()
        {
            HttpContext = context
        };

        var result = await controller.CheckBlock(ip) as OkObjectResult;

        Assert.NotNull(result);

        var response = Assert.IsType<BlockedAttemptDto>(result.Value);
        Assert.Equal("8.8.8.8", response.IpAddress);
        Assert.Equal("US", response.CountryCode);
        Assert.True(response.IsBlocked);
        Assert.Equal("UnitTestAgent", response.UserAgent);



        mockAttemptsRepo.Verify(r => r.AddAttempt(It.IsAny<BlockedAttempt>()), Times.Once);
    }

    [Fact]
    public async Task CheckBlock_ReturnsNotBlockedStatus_WhenCountryIsNotBlocked()
    {
        var ip = "8.8.4.4";
        var ipInfo = new IpInfo { CountryCode = "DE", Country = "Germany" };

        var mockIpLookupService = new Mock<IIpLookupService>();
        mockIpLookupService.Setup(s => s.GetIpInfoAsync(ip)).ReturnsAsync(ipInfo);

        var mockBlockedRepo = new Mock<IBlockedCountryRepository>();
        mockBlockedRepo.Setup(r => r.Exists("DE")).Returns(false);

        var mockAttemptsRepo = new Mock<IBlockedAttemptsRepository>();

        var controller = new IpController(mockIpLookupService.Object, mockAttemptsRepo.Object, mockBlockedRepo.Object);
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(ip);
        context.Request.Headers["User-Agent"] = "UnitTest";
        controller.ControllerContext = new ControllerContext { HttpContext = context };

        var result = await controller.CheckBlock(ip) as OkObjectResult;

        dynamic response = result!.Value!;
        Assert.Equal(ip, response.IpAddress);
        Assert.Equal("DE", response.CountryCode);
        Assert.False(response.IsBlocked);

        mockAttemptsRepo.Verify(r => r.AddAttempt(It.IsAny<BlockedAttempt>()), Times.Once);
    }




    [Fact]
    public async Task CheckBlock_ReturnsServerError_WhenIpLookupFails()
    {
        var ip = "1.2.3.4";

        var mockIpLookupService = new Mock<IIpLookupService>();
        mockIpLookupService.Setup(s => s.GetIpInfoAsync(ip)).ReturnsAsync((IpInfo?)null);

        var mockBlockedRepo = new Mock<IBlockedCountryRepository>();
        var mockAttemptsRepo = new Mock<IBlockedAttemptsRepository>();

        var controller = new IpController(mockIpLookupService.Object, mockAttemptsRepo.Object, mockBlockedRepo.Object);
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(ip);
        controller.ControllerContext = new ControllerContext { HttpContext = context };

        var result = await controller.CheckBlock(ip) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal(500, result!.StatusCode);
        Assert.Equal("Failed to retrieve IP info.", result.Value);
    }

    [Fact]
    public async Task CheckBlock_ReturnsBadRequest_WhenIpIsNullAndRemoteIpIsNull()
    {
        var mockIpLookupService = new Mock<IIpLookupService>();
        var mockBlockedRepo = new Mock<IBlockedCountryRepository>();
        var mockAttemptsRepo = new Mock<IBlockedAttemptsRepository>();

        var controller = new IpController(mockIpLookupService.Object, mockAttemptsRepo.Object, mockBlockedRepo.Object);
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = null;
        controller.ControllerContext = new ControllerContext { HttpContext = context };


        var result = await controller.CheckBlock(null) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result!.StatusCode);
        Assert.Equal("Unable to resolve caller IP address.", result.Value);
    }


}
