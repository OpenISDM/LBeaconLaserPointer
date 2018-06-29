# LBeaconLaserPointer
LBeacon雷射定位儀，透過2個參考點座標使用雷射筆指向目標座標，使工人能夠準確安裝LBeacon至正確位置

## 使用方法
**LBeacon安裝資訊同步**

將雷射定位儀連接至網際網路，主畫面->下載資料->同步，此時會開啟鏡頭，使用電腦或手機開啟LBeacon資訊網頁，登入網頁選擇伺服器資訊，將鏡頭對準網頁下方的QRcode，等待安裝資訊同步完成

**開始定位**

主畫面->定位->選擇安裝地點(臺大醫院、台北市政府)，此時會開啟鏡頭，掃描LBeacon蓋子上方的QRcode，雷射定位儀會自動載入安裝位置相關資訊顯示於畫面上，工人確認無誤後點選START，機器就會自動轉動使用雷射筆指向要安裝的位置

**清除本地資料**

主畫面->下載資料->清除

## 注意事項
- 雷射定位儀上方不能為深色物體
- 執行安裝任務前須同步資料
- 將定位儀放至平面圖參考點位置，並且面朝平面圖標示之位置

## 建置環境

- [Raspberry Pi 3](https://www.raspberrypi.com.tw/)
- [Windows IOT](https://developer.microsoft.com/zh-tw/windows/iot)
- [Visual Studio](https://visualstudio.microsoft.com)
- [UWP](https://docs.microsoft.com/zh-tw/windows/uwp/get-started/universal-application-platform-guide)
