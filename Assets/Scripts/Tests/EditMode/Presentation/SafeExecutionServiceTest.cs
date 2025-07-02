using Presentation.Services;

using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.EditMode.Presentation
{
    public sealed class SafeExecutionServiceTest
    {
        private int _executionCount;
        private Exception _lastCaughtException;

        [SetUp]
        public void SetUp()
        {
            _executionCount = 0;
            _lastCaughtException = null;
        }

        [Test]
        public void SafeExecute_ShouldExecuteAction_WhenActionIsValid()
        {
            // Arrange
            var executed = false;

            // Act
            SafeExecutionService.SafeExecute(() => executed = true);

            // Assert
            Assert.IsTrue(executed);
        }

        [Test]
        public void SafeExecute_ShouldHandleException_WhenActionThrows()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Test exception");

            // Act
            SafeExecutionService.SafeExecute(
                () => throw expectedException,
                ex => _lastCaughtException = ex
            );

            // Assert
            Assert.IsNotNull(_lastCaughtException);
            Assert.AreEqual(expectedException, _lastCaughtException);
        }

        [Test]
        public void SafeExecute_ShouldNotThrow_WhenActionIsNull()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => SafeExecutionService.SafeExecute(null));
        }

        [UnityTest]
        public IEnumerator SafeExecuteAsync_ShouldExecuteTask_WhenTaskIsValid() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var executed = false;

            // Act
            await SafeExecutionService.SafeExecuteAsync(
                () =>
                {
                    executed = true;
                    return UniTask.CompletedTask;
                },
                CancellationToken.None
            );

            // Assert
            Assert.IsTrue(executed);
        });

        [UnityTest]
        public IEnumerator SafeExecuteAsync_ShouldHandleException_WhenTaskThrows() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var expectedException = new InvalidOperationException("Test async exception");

            // Act
            await SafeExecutionService.SafeExecuteAsync(
                () => throw expectedException,
                CancellationToken.None,
                ex => _lastCaughtException = ex
            );

            // Assert
            Assert.IsNotNull(_lastCaughtException);
            Assert.AreEqual(expectedException, _lastCaughtException);
        });

        [UnityTest]
        public IEnumerator SafeExecuteAsync_ShouldRethrowOperationCanceledException() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await SafeExecutionService.SafeExecuteAsync(
                    () => UniTask.FromCanceled(cts.Token),
                    cts.Token
                );
            });
        });

        [UnityTest]
        public IEnumerator SafeExecuteWithRetryAsync_ShouldRetryOnFailure() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            const int maxRetries = 2;
            _executionCount = 0;

            // Act
            await SafeExecutionService.SafeExecuteWithRetryAsync(
                () =>
                {
                    _executionCount++;
                    if (_executionCount < 3) // Fail twice, succeed on third try
                    {
                        throw new InvalidOperationException("Retry test exception");
                    }
                    return UniTask.CompletedTask;
                },
                maxRetries,
                CancellationToken.None,
                ex => _lastCaughtException = ex
            );

            // Assert
            Assert.AreEqual(3, _executionCount); // Should execute 3 times (initial + 2 retries)
        });

        [UnityTest]
        public IEnumerator SafeExecuteWithRetryAsync_ShouldStopAfterMaxRetries() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            const int maxRetries = 2;
            _executionCount = 0;

            // Act
            await SafeExecutionService.SafeExecuteWithRetryAsync(
                () =>
                {
                    _executionCount++;
                    throw new InvalidOperationException("Always fail exception");
                },
                maxRetries,
                CancellationToken.None,
                ex => _lastCaughtException = ex
            );

            // Assert
            Assert.AreEqual(3, _executionCount); // Should execute 3 times (initial + 2 retries)
            Assert.IsNotNull(_lastCaughtException);
            Assert.AreEqual("Always fail exception", _lastCaughtException.Message);
        });

        [UnityTest]
        public IEnumerator SafeExecuteWithRetryAsync_ShouldSucceedOnFirstTry() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            const int maxRetries = 3;
            _executionCount = 0;

            // Act
            await SafeExecutionService.SafeExecuteWithRetryAsync(
                () =>
                {
                    _executionCount++;
                    return UniTask.CompletedTask; // Always succeed
                },
                maxRetries,
                CancellationToken.None
            );

            // Assert
            Assert.AreEqual(1, _executionCount); // Should execute only once
            Assert.IsNull(_lastCaughtException);
        });

        [UnityTest]
        public IEnumerator SafeExecuteWithRetryAsync_ShouldRethrowOperationCanceledException() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await SafeExecutionService.SafeExecuteWithRetryAsync(
                    () => UniTask.FromCanceled(cts.Token),
                    3,
                    cts.Token
                );
            });
        });

        [TearDown]
        public void TearDown()
        {
            _executionCount = 0;
            _lastCaughtException = null;
        }
    }
}