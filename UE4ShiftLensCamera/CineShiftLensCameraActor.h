// Fill out your copyright notice in the Description page of Project Settings.
/*	Created by christinayan01 by Takahiro Yanai. 2020.10.4
	https://christinayan01.jp/architecture/ | ARCHITECTURE GRAVURE
 */

#pragma once

#include "CoreMinimal.h"
#include "CineCameraActor.h"
#include "CineShiftLensCameraActor.generated.h"

/**
 * �V�t�g�����Y�@�\�t���V�l�}�J�����A�N�^�[
 */
UCLASS(ClassGroup = Common, hideCategories = (Input, Rendering, AutoPlayerActivation), showcategories = ("Input|MouseInput", "Input|TouchInput"), Blueprintable)
class POLYGON_API ACineShiftLensCameraActor : public ACineCameraActor
{
	GENERATED_BODY()

public:
	ACineShiftLensCameraActor(const FObjectInitializer& ObjectInitializer);

	virtual void Tick(float DeltaTime) override;

	// �V�t�g�����Y�l. ���J�p�����[�^
	UPROPERTY(Interp, BlueprintSetter = SetShiftLens, EditAnywhere, BlueprintReadWrite, Category = "Current Camera Settings", meta = (UIMin = "-1.0", UIMax = "1.0"))
		float ShiftLens;
	UFUNCTION(BlueprintCallable, BlueprintSetter, Category = "Cine Camera")
		void SetShiftLens(const float& InShiftLens);

	// �V�t�g�����Y�J�����̍X�V
	UFUNCTION(BlueprintCallable, Category = "Cine Camera")
		void UpdateShiftLens();

};
