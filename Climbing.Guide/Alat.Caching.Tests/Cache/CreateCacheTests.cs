﻿using System;
using System.Threading;
using Xunit;

namespace Alat.Caching.Tests.Cache {
   public class CreateCacheTests {
      private CacheStore Store { get; }

      public CreateCacheTests() {
         Store = new Mocks.CacheStoreMock(Array.Empty<CacheItem>());
      }

      [Fact]
      public void Default() {
         Assert.NotNull(new Impl.Cache(Store));
      }

      [Fact]
      public void NullStore() {
         Assert.Throws<ArgumentNullException>(() => new Impl.Cache(null));
      }

      [Fact]
      public void NegativeCleanInterval() {
         Assert.NotNull(new Impl.Cache(Store, new TimeSpan(-1)));
      }

      [Fact]
      public void ZeroCleanInterval() {
         Assert.NotNull(new Impl.Cache(Store, TimeSpan.Zero));
      }

      [Fact]
      public void MaxCleanInterval() {
         Assert.NotNull(new Impl.Cache(Store, TimeSpan.MaxValue));
      }

      [Fact]
      public void InfiniteCleanInterval() {
         Assert.NotNull(new Impl.Cache(Store, Timeout.InfiniteTimeSpan));
      }
   }
}
