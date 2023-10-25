﻿namespace Common.ResourceClasses;
public class CapsuleResource : ModelResource
{
    private static readonly Lazy<CapsuleResource> InternalResource = new(() => new CapsuleResource());

    public static CapsuleResource Instance => InternalResource.Value;

    public CapsuleResource() : base(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/BoundingShapes/Capsule.dae"))
    { }
}
