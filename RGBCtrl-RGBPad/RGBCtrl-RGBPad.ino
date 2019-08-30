//鱼咸
//190810
//RGBCtrl
//设置灯数量和引脚只能重新编译

//通过串口控制,9600bps 换行符结束，命令用例：
//uname= ID显示
//on=0 全开,缓存默认白色
//rgb=0,233,255 设颜色到缓存，青色
//html=#00ffff 设颜色到缓存，青色
//on=3 开灯3，用缓存颜色
//off=0 全关
//clear= 缓存颜色全亮
//left=n  左移n个灯，n>0
//right=n  右移n个灯，n>0
//run=0 停止流水
//run=1 流水灯效1

//命令格式上述[name=value] 命令name 和值value

const char devMsgName[] = "RGB Pad";
const char devMsgError[] = "Error";
const char devMsgOk[] = "OK";
String inputString = "";         // a String to hold incoming data
boolean stringComplete = false;  // whether the string is complete
//rgb
#include <NeoPixelBus.h>
#include <NeoPixelAnimator.h>
const uint16_t PixelCount = 16; // this example assumes 4 pixels, making it smaller will cause a failure
uint8_t PixelPin = A1;  // make sure to set this to the correct pin, ignored for Esp8266
#define colorSaturation 128
// three element pixels, in different order and speeds
NeoPixelBus<NeoGrbFeature, Neo800KbpsMethod> strip(PixelCount, PixelPin);
RgbColor white(colorSaturation);
RgbColor black(0);
HsbColor color(0, 1, 1);
//run
int runEn = 0;
//-1随机颜色流 1转圈模式 2呼吸 3循环变色 0关
#define RGB_RANDOM -1
#define RGB_ROAD 1
#define RGB_FEAD 2
#define RGB_COLOR 3
#define RGB_OFF_ECHO -2

#include <EEPROM.h>
/*
  SerialEvent occurs whenever a new data comes in the hardware serial RX. This
  routine is run between each time loop() runs, so using delay inside loop can
  delay response. Multiple bytes of data may be available.
*/
void serialEvent() {
  while (Serial.available()) {
    // get the new byte:
    char inChar = (char)Serial.read();
    // add it to the inputString:
    inputString += inChar;
    // if the incoming character is a newline, set a flag so the main loop can
    // do something about it:
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
}

void setup() {
  // initialize serial:
  Serial.begin(9600);
  // reserve 200 bytes for the inputString:
  inputString.reserve(200);

  // this resets all the neopixels to an off state
  strip.Begin();
  strip.Show();

  //eeprom
  for (int i = 0; i < PixelCount; i++)
  {
    HsbColor colorthis = takeLight(i);
    strip.SetPixelColor(i, colorthis);
  }
  strip.Show();
  run(EEPROM.read(0));

  //  delay(10000);
}

void loop() {
  // print the string when a newline arrives:
  serialEvent();
  if (stringComplete) {
    cmdPrcessing(inputString);
    // clear the string:
    inputString = "";
    stringComplete = false;
  }
  if (runEn) {
    runRGB();
  }
  else
    delay(200);
}
void cmdPrcessing(String & args) {
  //分割
  //  str = Serial.readString();
  int index = args.indexOf('=');
  String cmd = args.substring(0, index);
  String msg = args.substring(index + 1, args.length());
  //进入命令
  if (cmd == "uname")
  {
    Serial.println(devMsgName);
  }
  else if (cmd == "on")
  {
    //使用数据
    //  int x = str.substring(0, index).toInt();
    //  int y = str.substring(index + 1, str.length()).toInt();
    int value = msg.toInt();
    setLightOn(value);
    Serial.println(devMsgOk);
  }
  else if (cmd == "off")
  {
    //使用数据
    //  int x = str.substring(0, index).toInt();
    //  int y = str.substring(index + 1, str.length()).toInt();
    int value = msg.toInt();
    setLightOff(value);
    Serial.println(devMsgOk);
  }
  else if (cmd == "num")
  {
    int value = msg.toInt();
    //PixelCount = (value);
    initRGB();
    Serial.println(devMsgOk);
  }
  else if (cmd == "pin")
  {
    int value = msg.toInt();
    PixelPin = (value);
    initRGB();
    Serial.println(devMsgOk);
  }
  else if (cmd == "color")
  {
    setColor(msg);
    Serial.println(devMsgOk);
  }
  else if (cmd == "rgb")
  {
    setColor(msg);
    Serial.println(devMsgOk);
  }
  //setRGBW
  else if (cmd == "rgbw")
  {
    setRGBW(msg);
    Serial.println(devMsgOk);
  }
  //setHTML
  else if (cmd == "html")
  {
    setHTML(msg);
    Serial.println(devMsgOk);
  }
  else if (cmd == "hsbh")
  {
    setHsbh(msg);
    Serial.println(devMsgOk);
  }
  else if (cmd == "hsbs")
  {
    setHsbs(msg);
    Serial.println(devMsgOk);
  }
  else if (cmd == "hsbb")
  {
    setHsbb(msg);
    Serial.println(devMsgOk);
  }
  else if (cmd == "left")
  {
    int value = msg.toInt();
    left(value);
    Serial.println(devMsgOk);
  }
  else if (cmd == "right")
  {
    int value = msg.toInt();
    right(value);
    Serial.println(devMsgOk);
  }
  else if (cmd == "clear")
  {
    clear();
    Serial.println(devMsgOk);
  }
  else if (cmd == "run")
  {
    int value = msg.toInt();
    run(value);
    Serial.println(devMsgOk);
  }
  else if (cmd == "light")
  {
    int value = msg.toInt();
    if (value == 0) {
      setLightOff(0);
    }
    else {
      for (int i = 0; i < PixelCount; i++)
      {
        HsbColor colorthis = HsbColor((float)(EEPROM.read(i * 3 + 1)) / 100, (float)(EEPROM.read(i * 3 + 2)) / 100, value / 100);
        strip.SetPixelColor(i, colorthis);
      }
      strip.Show();
    }
    Serial.println(devMsgOk);
  }
  else
  {
    Serial.println(devMsgError);
    return;
  }

}
int lastLedIndex = 0;
void setLightOn(int index) {
  //  Serial.println(index);
  if (index == 0)
  {
    //all
    for (int i = 0; i < PixelCount; i++)
    {
      saveLight(i);
      strip.SetPixelColor(i, color);
    }
  }
  else
  {
    int i = index - 1;
    if (runEn == 0)
      lastLedIndex = i;
    saveLight(i);
    strip.SetPixelColor(index - 1, color);
  }
  strip.Show();
}
void setLightOff(int index) {
  //  Serial.println(index);
  if (index == 0)
  {
    //all
    for (int i = 0; i < PixelCount; i++)
    {
      if (runEn == 0)
        EEPROM.update(i * 3 + 3, 0);
      strip.SetPixelColor(i, black);
    }
  }
  else
  {
    int i = index - 1;
    if (runEn == 0)
      EEPROM.update(i * 3 + 3, 0);
    strip.SetPixelColor(index - 1, black);
  }
  strip.Show();
}
NeoPixelBus<NeoGrbFeature, Neo800KbpsMethod> initNeoPixelBus() {
  return NeoPixelBus<NeoGrbFeature, Neo800KbpsMethod>(PixelCount, PixelPin);
}
void initRGB() {
  //  strip = new NeoPixelBus<NeoGrbFeature, Neo800KbpsMethod>(PixelCount, PixelPin);
  //  strip = initNeoPixelBus();
}
RgbColor buildColor(byte R, byte G, byte B)
{
  return RgbColor(G, R, B);
}
String findArgsBy(String& data, char flag, int index)
{
  int j = 0, k = 0;
  for (int i = 0; i <= index; i++)
  {
    if (k != 0)
      j = k + 1; //上一个flag位置存j，找下一个
    k = data.indexOf(flag, j);//j开始找下一个flag存k
    if (k == -1)
    {
      return data.substring(j);
    }
  }

  return data.substring(j, k);
}
void setColor(String & atVal) {
  color = RgbColor(
            atoi(findArgsBy(atVal, ',', 0).c_str())
            , atoi(findArgsBy(atVal, ',', 1).c_str())
            , atoi(findArgsBy(atVal, ',', 2).c_str())
          );
}
//error fail
void setRGBW(String & atVal) {
  color = RgbColor(
            atoi(findArgsBy(atVal, ',', 0).c_str())
            , atoi(findArgsBy(atVal, ',', 1).c_str())
            , atoi(findArgsBy(atVal, ',', 2).c_str())
            //  , atoi(findArgsBy(atVal, ',', 3).c_str())
          );
}
void setHTML(String & atVal) {
  //atVal=atVal.trim();
  HtmlColor newColor = HtmlColor();
  newColor.Parse<HtmlColorNames>(atVal);
  color = RgbColor(newColor);
  //  char buf[50]={0};
  //  color.ToNumericalString(buf,50);
  //  Serial.println(buf);

}


void setHsbh(String & atVal) {
  float value = (float)(atVal.toInt()) / 100;
  color.H = value;
  setLastLight();
  //  color = HsbColor(value, color.S, color.B);
}
void setHsbs(String & atVal) {
  float value =  (float)(atVal.toInt()) / 100;
  color.S = value;
  setLastLight();
  //  color = HsbColor(color.H, value, color.B);
}
void setHsbb(String & atVal) {
  float value =  (float)(atVal.toInt()) / 100;
  color.B = value;
  setLastLight();
  //  color = HsbColor(color.H, color.S, value);
}

void clear() {
  strip.ClearTo(color);
  strip.Show();
}
void clearblack() {
  strip.ClearTo(black);
  strip.Show();
}

void left(int num) {

  strip.ShiftLeft(num);
  strip.Show();
}
void right(int num) {

  strip.ShiftRight(num);
  strip.Show();
}

//run

const uint16_t AnimCount = PixelCount / 5 * 4 + 1; // we only need enough animations for the tail and one extra

const uint16_t PixelFadeDuration = 300; // third of a second
// one second divide by the number of pixels = loop once a second
const uint16_t NextPixelMoveDuration = 1000 / PixelCount; // how fast we move through the pixels

NeoGamma<NeoGammaTableMethod> colorGamma; // for any fade animations, best to correct gamma

// what is stored for state is specific to the need, in this case, the colors and
// the pixel to animate;
// basically what ever you need inside the animation update function
struct MyAnimationState
{
  RgbColor StartingColor;
  RgbColor EndingColor;
  uint16_t IndexPixel; // which pixel this animation is effecting
};

NeoPixelAnimator animations(AnimCount); // NeoPixel animation management object
MyAnimationState animationState[AnimCount];
uint16_t frontPixel = 0;  // the front of the loop
RgbColor frontColor;  // the color at the front of the loop


void FadeOutAnimUpdate(const AnimationParam& param)
{
  // this gets called for each animation on every time step
  // progress will start at 0.0 and end at 1.0
  // we use the blend function on the RgbColor to mix
  // color based on the progress given to us in the animation
  RgbColor updatedColor = RgbColor::LinearBlend(
                            animationState[param.index].StartingColor,
                            animationState[param.index].EndingColor,
                            param.progress);
  // apply the color to the strip
  strip.SetPixelColor(animationState[param.index].IndexPixel,
                      colorGamma.Correct(updatedColor));
}

void LoopAnimUpdate(const AnimationParam& param)
{
  // wait for this animation to complete,
  // we are using it as a timer of sorts
  if (param.state == AnimationState_Completed)
  {
    // done, time to restart this position tracking animation/timer
    animations.RestartAnimation(param.index);

    // pick the next pixel inline to start animating
    //
    frontPixel = (frontPixel + 1) % PixelCount; // increment and wrap
    if (frontPixel == 0)
    {
      // we looped, lets pick a new front color
      frontColor = HslColor(random(360) / 360.0f, 1.0f, 0.25f);
      if (runEn == RGB_ROAD)
        frontColor = color;
    }

    uint16_t indexAnim;
    // do we have an animation available to use to animate the next front pixel?
    // if you see skipping, then either you are going to fast or need to increase
    // the number of animation channels
    if (animations.NextAvailableAnimation(&indexAnim, 1))
    {
      animationState[indexAnim].StartingColor = frontColor;
      animationState[indexAnim].EndingColor = RgbColor(0, 0, 0);
      animationState[indexAnim].IndexPixel = frontPixel;

      animations.StartAnimation(indexAnim, PixelFadeDuration, FadeOutAnimUpdate);
    }
  }
}




void run(int seed) {
  EEPROM.update(0, seed);
  clearblack();
  if (seed == 0)
  {
    //stop
    runEn = 0;
    for (int i = 0; i < PixelCount; i++)
    {
      HsbColor colorthis = takeLight(i);
      strip.SetPixelColor(i, colorthis);
    }
    strip.Show();
    color = HsbColor(0, 1, 1);
  }
  else {
    //init run
    if (seed == RGB_ROAD) {
      runEn = RGB_ROAD;
      animations.StartAnimation(0, NextPixelMoveDuration, LoopAnimUpdate);
    }
    else if (seed == RGB_FEAD) {
      runEn = RGB_FEAD;
      color.H = 0;
      color.S = 1;
      color.B = 1;
    }
    else if (seed == RGB_COLOR) {
      runEn = RGB_COLOR;
      color.H = 0.5;
      color.S = 1;
      color.B = 1;
    }
    else {
      runEn = RGB_RANDOM;
      // ATPrinter.println(seed);
      randomSeed(seed);
      // we use the index 0 animation to time how often we move to the next
      // pixel in the strip
      animations.StartAnimation(0, NextPixelMoveDuration, LoopAnimUpdate);
    }
  }
}
//run do
void runRGB() {
  //  if (runEn == RGB_ROAD) {
  //  }
  //  else
  if (runEn == RGB_FEAD) {
    static int feadH = 0;
    static int  setp = 3;
    feadH += setp;
    if (feadH < 80 && feadH > 0)
    {
    }
    else if (feadH >= 80) {
      setp = -setp;
      feadH = 80;
    }
    else {
      setp = -setp;
      feadH = 0;
    }
    //color.H = feadH / 100;
    //    color = HsbColor(color.H , color.S, (float)feadH / 100);
    color.B =  (float)feadH / 100;
    //    Serial.print(feadH);
    //    Serial.print("   ");
    //    Serial.println(color.B);
    setLightOn(0);
    delay(30);
  }
  else if (runEn == RGB_COLOR) {
    static int feadH = 0;
    if (feadH < 100)
    {
      feadH++;
    }
    else
      feadH = 0;
    color.H = (float)feadH / 100;
    //    color = HsbColor((float)feadH / 100, color.S, color.B);

    setLightOn(0);
    delay(50);
  }
  else {
    // this is all that is needed to keep it running
    // and avoiding using delay() is always a good thing for
    // any timing related routines
    animations.UpdateAnimations();
    strip.Show();
  }
}
void saveLight(int i) {
  if (runEn == 0) {
    EEPROM.update(i * 3 + 1, (char)(color.H * 100)), EEPROM.update(i * 3 + 2, (char)(color.S * 100)), EEPROM.update(i * 3 + 3, (char)(color.B * 100));
    //    Serial.print("saveLight:");
    //    Serial.print(i); Serial.print("   ");
    //    Serial.print(color.H); Serial.print("   ");
    //    Serial.print(color.S); Serial.print("   ");
    //    Serial.println(color.B);

  }
}
HsbColor takeLight(int i) {
  return HsbColor((float)(EEPROM.read(i * 3 + 1)) / 100, (float)(EEPROM.read(i * 3 + 2)) / 100, (float)(EEPROM.read(i * 3 + 3)) / 100);
}
void setLastLight() {
  if (runEn == 0) {
    //    strip.SetPixelColor(lastLedIndex, color);
    //    strip.Show();
    setLightOn(lastLedIndex + 1);
  }
}
void setLightOf(int index) {
}

