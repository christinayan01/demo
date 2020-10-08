// Fill out your copyright notice in the Description page of Project Settings.


#include "ShiftLensCameraComponent.h"
#include "Engine.h"

UShiftLensCameraComponent::UShiftLensCameraComponent(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	ShiftLens = 0.0f;
}

void UShiftLensCameraComponent::GetCameraView(float DeltaTime, FMinimalViewInfo& DesiredView)
{
	Super::GetCameraView(DeltaTime, DesiredView);
	DesiredView.OffCenterProjectionOffset.Y = this->ShiftLens;
}
