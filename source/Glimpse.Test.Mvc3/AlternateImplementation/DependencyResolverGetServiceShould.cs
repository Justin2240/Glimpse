﻿using System.Web.Mvc;
using Glimpse.Core;
using Glimpse.Core.Extensibility;
using Glimpse.Test.Common;
using Moq;
using Xunit;
using Xunit.Extensions;
using DependencyResolver = Glimpse.Mvc.AlternateImplementation.DependencyResolver;

namespace Glimpse.Test.Mvc3.AlternateImplementation
{
    public class DependencyResolverGetServiceShould
    {
        [Fact]
        public void Construct()
        {
            var sut = new DependencyResolver.GetService();

            Assert.NotNull(sut.MethodToImplement);
        }

        [Theory, AutoMock]
        public void ImplementGetService(DependencyResolver.GetService sut)
        {
            var methodToImplement = sut.MethodToImplement;

            Assert.Equal("GetService", methodToImplement.Name);
        }

        [Theory, AutoMock]
        public void ProceedWithRuntimePolicyOff(DependencyResolver.GetService sut, IAlternateImplementationContext context)
        {
            context.Setup(c => c.RuntimePolicyStrategy).Returns(() => RuntimePolicy.Off);

            sut.NewImplementation(context);

            context.Verify(c => c.Proceed());
            context.Verify(c => c.ReturnValue, Times.Never());
            context.Verify(c => c.Arguments, Times.Never());
        }

        [Theory, AutoMock]
        public void PublishMessageWithRuntimePolicyOn(DependencyResolver.GetService sut, IAlternateImplementationContext context)
        {
            context.Setup(c => c.Arguments).Returns(new object[] { typeof(IController) });

            sut.NewImplementation(context);

            context.Verify(c => c.Proceed());
            context.MessageBroker.Verify(b => b.Publish(It.IsAny<DependencyResolver.GetService.Message>()));
        }
    }
}