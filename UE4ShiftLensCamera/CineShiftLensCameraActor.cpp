// Fill out your copyright notice in the Description page of Project Settings.
// Created by christinayan01 by Takahiro Yanai. 2020.10.4


#include "CineShiftLensCameraActor.h"
#include "Engine.h"
#include "YanaiLocalPlayer.h"

//
ACineShiftLensCameraActor::ACineShiftLensCameraActor(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	SetActorTickEnabled(true);

	this->ShiftLens = 0.0f;
}

//
void ACineShiftLensCameraActor::SetShiftLens(const float& InShiftLens) 
{ 
	this->ShiftLens = InShiftLens; 
}

//
void ACineShiftLensCameraActor::Tick(float DeltaTime)
{
	UpdateShiftLens();
	Super::Tick(DeltaTime);
}

// �V�t�g�����Y�J�����̍X�V
void ACineShiftLensCameraActor::UpdateShiftLens()
{
	APlayerController* PlayerController = nullptr;

	for (FConstPlayerControllerIterator Iterator = GetWorld()->GetPlayerControllerIterator(); Iterator; ++Iterator)
	{
		PlayerController = Iterator->Get();
		if (PlayerController && PlayerController->PlayerCameraManager)
		{
			AActor* ViewTarget = PlayerController->GetViewTarget();
			if (ViewTarget == this) 	// �b��F�r���[�|�[�g�\���Ɏg�p���Ă���J�������������ł���
			{
				ULocalPlayer* LocalPlayer = PlayerController->GetLocalPlayer();
				if (LocalPlayer) 
				{
					UYanaiLocalPlayer* YanaiLocal = Cast<UYanaiLocalPlayer>(LocalPlayer);
					if (YanaiLocal) 
					{
						YanaiLocal->SetShiftLens(this->ShiftLens);
					}
				}
			}
		}
	}
}