using Presentation.DTO;
using Presentation.View.TitleScene;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace Presentation.State.TitleScene
{
    // TitleScene Specific ModalState Handlers
    public class TitleNoneStateHandler : TitleSceneModalStateHandlerBase
    {
        protected override async UniTask ApplyModalAsync(
            TitleSceneView view,
            TitleSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the none state.", ex);
            }
        }
    }

    public class UserNameInputStateHandler : TitleSceneModalStateHandlerBase
    {
        protected override async UniTask ApplyModalAsync(
            TitleSceneView view,
            TitleSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.ShowPanel();
                view.UserNameModalView.ShowModal();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the user name input state.", ex);
            }
        }
    }

    public class LicenseStateHandler : TitleSceneModalStateHandlerBase
    {
        private bool _isLicenseTextSet = false;

        protected override async UniTask ApplyModalAsync(
            TitleSceneView view,
            TitleSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.ShowPanel();
                var licenseModalView = view.LicenseModalView;
                licenseModalView.ShowModal();

                // License word setting
                if (!_isLicenseTextSet)
                {
                    var licenseDtos = data.Licenses.Select(license => new LicenseDto(
                        $"{license.name}\n" +
                        $"{license.type}\n" +
                        $"{license.copyright}\n" +
                        "\n" +
                        string.Join("\n", license.terms.Select(term => $"{term}"))
                    )).ToList();
                    await licenseModalView.SetLicensesAsync(licenseDtos, ct);
                    _isLicenseTextSet = true;
                }
                // Layout adjustment, etc.
                licenseModalView.ForceMeshUpdateText();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the license state.", ex);
            }
        }
    }
}
