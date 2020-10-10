// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "CineCameraComponent.h"
#include "ShiftLensCineCameraComponent.generated.h"

/**
 * 
 */
UCLASS(HideCategories = (Mobility, Rendering, LOD), Blueprintable, ClassGroup = Camera, meta = (BlueprintSpawnableComponent))
class SHIFTLENS1_API UShiftLensCineCameraComponent : public UCineCameraComponent
{
	GENERATED_BODY()

public:
	virtual void GetCameraView(float DeltaTime, FMinimalViewInfo& DesiredView) override;

	UShiftLensCineCameraComponent();

	// シフトレンズのパラメータ
	UPROPERTY(Interp, BlueprintSetter = SetShiftLens, EditAnywhere, BlueprintReadWrite, Category = "Current Camera Settings")
		float ShiftLens;
	UFUNCTION(BlueprintCallable, Category = "Current Camera Settings")
		virtual void SetShiftLens(float InShiftLens);

	UPROPERTY(Interp, EditAnywhere, BlueprintReadWrite, Category = "Current Camera Settings")
		uint8 KeepHorizon : 1;

	// シフトレンズカメラの更新
	UFUNCTION(BlueprintCallable, Category = "Current Camera Settings")
		void UpdateShiftLens();

private:
	float LocationZ;
};
