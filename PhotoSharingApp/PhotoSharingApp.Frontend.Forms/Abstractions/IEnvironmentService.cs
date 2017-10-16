using System;

namespace PhotoSharingApp.Forms.Abstractions
{
    public interface IEnvironmentService
    {
        bool IsRunningInRealWorld();
    }
}