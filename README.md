## 맵디자인 설명서

### _BeginGameManager_
* Bullet Speed 자유롭게 변경

### _TankScene_
* Hierarchy에서 SpawnPoints가 있는데 빈 EmptyObject를 생성 후 원하는 장소에 배치 후 Rotation을 알맞게 조절하고 InGameManager의 spawnPoints 배열에 삽입한다.
* 게임에 접속 후 모든 플레이어가 Ready Buttom을 눌러야만 게임이 시작된다.
* 게임 시간은 InGameManager의 PlayTime으로 조절 가능하다.
* 포탄 데미지는 InGameManager의 BulletDamage로 조절 가능하다.
* 점수 표기 시간은 UiManagerTank의 InitUI에서 조절 가능하다.
