#include <Core/Utils.h>
#include <Core/Application/Application.h>
#include <Core/Application/Product.h>
#include <Core/Application/CustomEvents.h>
#include <Core/UserInterface/UserInterface.h>
#include <Core/UserInterface/CommandDefinitions.h>
#include <Core/UserInterface/CommandDefinition.h>
#include <Fusion/Fusion/Design.h>
#include <Fusion/Components/Component.h>
#include <Fusion/Fusion/ModelParameters.h>
#include <Fusion/Fusion/ModelParameter.h>

#include <thread> 
#include <sstream>
#include <chrono>



#include <Core/CoreAll.h>
#include <Fusion/FusionAll.h>
#include <CAM/CAMAll.h>
#include <windows.h>
#include <iostream>
#include <list>
#include <vector>


#include <random>
#include <future>


using namespace adsk::core;
using namespace adsk::fusion;
using namespace adsk::cam;
using namespace std;

const string myCustomEvent = "MyCustomEventId1";

#define INSTANCES 1
#define PIPE_TIMEOUT 5000
#define BUFSIZE 4096

typedef struct
{
	OVERLAPPED oOverlap;
	HANDLE hPipeInst;
	TCHAR chRequest[BUFSIZE];
	DWORD cbRead;
	TCHAR chReply[BUFSIZE];
	DWORD cbToWrite;
	DWORD dwState;
	BOOL fPendingIO;
} PIPEINST, * LPPIPEINST;

future<bool> connectthread;

bool restartPipe();
void getpipeData();
void debug(string msgout);
bool forcefullyDisconnectPipe();

HANDLE hPipe, clientPosePipe;

DWORD i, dwWait, cbRet, dwErr;
BOOL fSuccess, fConnected = false;
LPCWSTR lpszPipename = L"\\\\.\\pipe\\FUSION360";
static int getpipedatastate = 0;
static bool donereading = false;
static vector<string> finaloutput;

static bool disconnect = false;

Ptr<Application> app;
Ptr<UserInterface> ui;
Ptr<CustomEvent> customEvent;

Ptr<Camera> cam;
Ptr<Point3D> eye;
Ptr<Point3D> target;
Ptr<Vector3D> upvector;
Ptr<Point3D> finalpt;
Ptr<Viewport> openedview;

Ptr<BoolValueCommandInput> transmode;
Ptr<IntegerSliderCommandInput> tiltsense_slider;
Ptr<IntegerSliderCommandInput> slidesense_slider;
bool defaulttransmode = false;
int defaulttiltsensevalue = 100;
int defaultslidesensevalue = 100;


static string output = "";
static char recmsg[1];
bool stopFlag;
default_random_engine generator;

uniform_int_distribution<int> distribution(1000, 10000);


class ThreadEventHandler : public CustomEventHandler
{
public:
	void notify(const Ptr<CustomEventArgs>& eventArgs) override
	{

		//string output = to_string(upvector->x()) + " || " + to_string(upvector->y()) + " || " + to_string(upvector->z());



	//ui->messageBox(output);

	}
} onCustomEvent_;
#pragma region view class

class Timer
{
private:
	atomic<bool> timerEnabled = false;
	atomic<int> timerInterval = 0;
	thread timerThread;
	void (*fcptr)();

public:
	Timer(int timeinterval, void (*funcptr)()) : timerThread(&internalfunc, this)
	{
		fcptr = funcptr;
		timerInterval = timeinterval;
		//timerEnabled = en;
		timerThread.detach();
	}
	static void internalfunc(Timer* obj) {
		obj->startInternal();
	}
	void enabled(bool enable) {
		timerEnabled = enable;
		if (timerEnabled)
		{
			timerThread = thread(&internalfunc, this);
			timerThread.detach();
		}

	}
	bool enabled() {
		return timerEnabled;
	}
	void setinterval(int interval) {
		timerInterval = interval;
		//timerEnabled = true;
		timerThread = thread(&internalfunc, this);
		timerThread.detach();
	}
	void startInternal() {
		int iii = timerInterval;
		while (timerEnabled)
		{
			if (iii == timerInterval)
			{
				Sleep(timerInterval);
				if (iii == timerInterval)
				{
					fcptr();
				}
			}
			else
			{
				break;
			}
		}

	}
	void start() {

	}
	void stop() {

	}

};

class Quaternion
{
public:
	Quaternion(float w, float x, float y, float z)
	{
		w_cmp = w;
		x_cmp = x;
		y_cmp = y;
		z_cmp = z;
	}

	Quaternion(Ptr<Point3D> ptQ)
	{
		w_cmp = 0;
		x_cmp = ptQ->x();
		y_cmp = ptQ->y();
		z_cmp = ptQ->z();
	}

	Quaternion conj()
	{
		Quaternion resultQ(
			getw(),
			-1 * getx(),
			-1 * gety(),
			-1 * getz()
		);
		return resultQ;
	}

	Quaternion normalize()
	{
		float len = length();
		Quaternion resultQ(
			getw() / len,
			getx() / len,
			gety() / len,
			getz() / len
		);
		return resultQ;
	}

	Quaternion multiply(Quaternion qt)
	{
		Quaternion resultQ(
			getw() * qt.getw() - getx() * qt.getx() - gety() * qt.gety() - getz() * qt.getz(),
			getw() * qt.getx() + getx() * qt.getw() + gety() * qt.getz() - getz() * qt.gety(),
			getw() * qt.gety() - getx() * qt.getz() + gety() * qt.getw() + getz() * qt.getx(),
			getw() * qt.getz() + getx() * qt.gety() - gety() * qt.getx() + getz() * qt.getw()
		);
		return resultQ;
	}

	float length() {
		return sqrt(w_cmp * w_cmp + x_cmp * x_cmp + y_cmp * y_cmp + z_cmp * z_cmp);
	}

	float getw() {
		return w_cmp;
	}

	float getx() {
		return x_cmp;
	}
	float gety() {
		return y_cmp;
	}
	float getz() {
		return z_cmp;
	}

	~Quaternion()
	{
	}

private:

	float w_cmp;
	float x_cmp;
	float y_cmp;
	float z_cmp;
};


class Vector3d
{
public:
	Vector3d() {

	}
	Vector3d(float x, float y, float z)
	{
		x_vec = x;
		y_vec = y;
		z_vec = z;
	}

	Vector3d(Ptr<Point3D> ptv)
	{
		x_vec = ptv->x();
		y_vec = ptv->y();
		z_vec = ptv->z();
	}

	Vector3d getvector() {

	}

	Vector3d cross(Vector3d cr) {
		Vector3d resultvec(
			(this->gety() * cr.getz()) - (this->getz() * cr.gety()),
			(this->getz() * cr.getx()) - (this->getx() * cr.getz()),
			(this->getx() * cr.gety()) - (this->gety() * cr.getx())
		);
		return resultvec;
	}

	Vector3d normalize() {
		float len = length();
		Vector3d resultvec(
			getx() / len,
			gety() / len,
			getz() / len
		);
		return resultvec;
	}

	static Vector3d pointstovector(Ptr<Point3D> frm, Ptr<Point3D> to) {
		Vector3d resultvec(
			frm->x() - to->x(),
			frm->y() - to->y(),
			frm->z() - to->z()
		);
		return resultvec;
	}
	static Vector3d getparallelvector(Ptr<Point3D> startpt, Vector3d vec) {
		Ptr<Point3D> terminalpt = Point3D::create(
			startpt->x() - vec.getx(),
			startpt->y() - vec.gety(),
			startpt->z() - vec.getz()
		);
		Vector3d resultvec = pointstovector(startpt, terminalpt);
		return resultvec;
	}
	Ptr<Point3D> gettermpoint(Ptr<Point3D> startpt) {
		Ptr<Point3D> terminalpt = Point3D::create(
			startpt->x() + getx(),
			startpt->y() + gety(),
			startpt->z() + getz()
		);
		return terminalpt;
	}

	Vector3d reflect() {
		Vector3d resultvec(
			-getx(),
			-gety(),
			-getz()
		);
		return resultvec;
	}

	Vector3d scale(float sf) {
		Vector3d resultvec(
			getx() * sf,
			gety() * sf,
			getz() * sf
		);
		return resultvec;
	}
	float length() {
		return sqrt(getx() * getx() + gety() * gety() + getz() * getz());
	}

	float getx() {
		return x_vec;
	}
	float gety() {
		return y_vec;
	}
	float getz() {
		return z_vec;
	}

	~Vector3d()
	{
	}


private:
	float x_vec;
	float y_vec;
	float z_vec;
};



class AhmsvilleDial_View
{
public:

	AhmsvilleDial_View()
	{

	}


	Ptr<Point3D> rotateviewby(float xang, float yang) {
		//calc eye to target vector
		Vector3d vecET = Vector3d::pointstovector(view_eye, view_target);
		Vector3d vecUP(view_upvector);
		//calc LEFT vector
		Vector3d vecLEFT = vecUP.cross(vecET);
		//calc rotation axis
		Vector3d vecXROTaxis = Vector3d::getparallelvector(view_target, vecLEFT);
		Vector3d vecYROTaxis = Vector3d::getparallelvector(view_target, vecUP);
		vecYROTaxis = vecYROTaxis.normalize();
		vecXROTaxis = vecXROTaxis.normalize();
		//create rotation quaternions
		Quaternion xrotQ(
			cos(xang / 2),
			sin(xang / 2) * vecXROTaxis.getx(),
			sin(xang / 2) * vecXROTaxis.gety(),
			sin(xang / 2) * vecXROTaxis.getz()
		);
		Quaternion xrotQinv = xrotQ.conj();
		Quaternion yrotQ(
			cos(yang / 2),
			sin(yang / 2) * vecYROTaxis.getx(),
			sin(yang / 2) * vecYROTaxis.gety(),
			sin(yang / 2) * vecYROTaxis.getz()
		);
		Quaternion yrotQinv = yrotQ.conj();
		Quaternion pointQ(view_eye);
		/************************  yQ*(xQ*ptQ*xQinv)*yQinv  ********************************************/
		Quaternion finalQ = yrotQ.multiply(xrotQ.multiply(pointQ).multiply(xrotQinv)).multiply(yrotQinv);


		rotateview_eye = Point3D::create(
			finalQ.getx(),
			finalQ.gety(),
			finalQ.getz()
		);

		/********calc new upvector********************/
		//find new left vector
		Vector3d vecNewLEFT = Vector3d::getparallelvector(rotateview_eye, vecLEFT);
		//find new forward vector
		Vector3d vecNewET = Vector3d::pointstovector(rotateview_eye, view_target);
		//cross product = new upvector
		Vector3d vecNewUP = vecNewET.cross(vecNewLEFT).normalize();
		rotateview_upvector = Vector3D::create(
			vecNewUP.getx(),
			vecNewUP.gety(),
			vecNewUP.getz()
		);

		return rotateview_eye;
	}

	void translateviewby(float xdist, float ydist) {

		Ptr<Point3D> finaleyept = view_eye;
		Ptr<Point3D> finaltargetpt = view_target;
		Vector3d vecET = Vector3d::pointstovector(finaleyept, finaltargetpt);//calc eye to target vector
		Vector3d vecUP(view_upvector);
		Vector3d vecDOWN = vecUP.reflect();
		Vector3d vecTARGUP = Vector3d::getparallelvector(finaltargetpt, vecUP);
		Vector3d vecTARGDOWN = vecTARGUP.reflect();
		Vector3d vecLEFT = vecUP.cross(vecET);//calc right vector
		Vector3d vecTARGLEFT = Vector3d::getparallelvector(finaltargetpt, vecLEFT);
		Vector3d vecRIGHT = vecLEFT.reflect();
		Vector3d vecTARGRIGHT = Vector3d::getparallelvector(finaltargetpt, vecRIGHT);
		vecRIGHT = vecRIGHT.normalize();
		vecTARGRIGHT = vecTARGRIGHT.normalize();
		vecLEFT = vecLEFT.normalize();
		vecTARGLEFT = vecTARGLEFT.normalize();



		if (xdist < 0)//left
		{
			//get unit left vector



			//scale LEFT vector to value
			vecLEFT = vecLEFT.scale(xdist * (-1)); //eye
			vecTARGLEFT = vecTARGLEFT.scale(xdist * (-1));//target
			//get termination point
			finaleyept = vecLEFT.gettermpoint(finaleyept);
			finaltargetpt = vecTARGLEFT.gettermpoint(finaltargetpt);
		}
		else if (xdist > 0)//right
		{
			//get unit right vector



			//scale RIGHT vector to value
			vecRIGHT = vecRIGHT.scale(xdist); //eye
			vecTARGRIGHT = vecTARGRIGHT.scale(xdist);//target
			//get termination point
			finaleyept = vecRIGHT.gettermpoint(finaleyept);
			finaltargetpt = vecTARGRIGHT.gettermpoint(finaltargetpt);
		}
		if (ydist < 0)//down
		{
			//get unit DOWN vector



			//scale DOWN vector to value
			vecDOWN = vecDOWN.scale(ydist * (-1)); //eye
			vecTARGDOWN = vecTARGDOWN.scale(ydist * (-1));//target
			//get termination point
			finaleyept = vecDOWN.gettermpoint(finaleyept);
			finaltargetpt = vecTARGDOWN.gettermpoint(finaltargetpt);
		}
		else if (ydist > 0)//up
		{
			//get unit UP vector



			//scale UP vector to value
			vecUP = vecUP.scale(ydist); //eye
			vecTARGUP = vecTARGUP.scale(ydist);//target
			//get termination point
			finaleyept = vecUP.gettermpoint(finaleyept);
			finaltargetpt = vecTARGUP.gettermpoint(finaltargetpt);
		}
		transview_eye = finaleyept;
		transview_target = finaltargetpt;
	}

	Ptr<Point3D> zoomby(float zf) {
		//get target to new eye vector
		Vector3d vecTE = Vector3d::pointstovector(view_eye, view_target);
		//get vector magnitude
		float mag = vecTE.length();
		//add to vector magnitude
		mag = mag + zf;
		Vector3d finalvecTE = vecTE.normalize();
		finalvecTE = finalvecTE.scale(mag);
		//get new eye point
		Ptr<Point3D> neweyept = finalvecTE.gettermpoint(view_target);
		return neweyept;
	}

	void setviewpoints(Ptr<Point3D> cam_eye, Ptr<Point3D> cam_target, Ptr<Vector3D> cam_upvector) {
		view_eye = cam_eye;
		view_target = cam_target;
		view_upvector = Point3D::create(cam_upvector->x(), cam_upvector->y(), cam_upvector->z());;
	}
	Ptr<Point3D> gettranslatedeye() {
		return transview_eye;
	}
	Ptr<Point3D> gettranslatedtarget() {
		return transview_target;
	}
	Ptr<Point3D> getrotatedeye() {
		return rotateview_eye;
	}
	Ptr<Vector3D> getrotateupvector() {
		return rotateview_upvector;
	}

	AhmsvilleDial_View::~AhmsvilleDial_View()
	{
	}
private:
	Ptr<Point3D> view_eye = Point3D::create(0, 0, 0);
	Ptr<Point3D> view_target = Point3D::create(0, 0, 0);
	Ptr<Point3D> view_upvector = Point3D::create(0, 0, 0);

	Ptr<Point3D> transview_eye = Point3D::create(0, 0, 0);
	Ptr<Point3D> transview_target = Point3D::create(0, 0, 0);
	Ptr<Point3D> rotateview_eye = Point3D::create(0, 0, 0);
	Ptr<Vector3D> rotateview_upvector;
};

void getpipeData() {

	LPVOID  lpBuffer;
	DWORD   nBufferSize;
	DWORD lpBytesRead;
	DWORD lpTotalBytesAvail;
	DWORD lpBytesLeftThisMessage;
	TCHAR singlebuff[1];

	string output = "";
	chrono::steady_clock::time_point t_start = chrono::high_resolution_clock::now();
	chrono::steady_clock::time_point t_end;
	double elapsed_time_ms = 0;
	while (elapsed_time_ms < 1000)
	{
		if (fConnected)
		{
			while (GetLastError() != ERROR_BROKEN_PIPE)
			{
				getpipedatastate = 1;
				t_start = chrono::high_resolution_clock::now();
				output = "";

				singlebuff[0] = ' ';

				while (singlebuff[0] != '*')
				{
					bool peakres = false;
					peakres = PeekNamedPipe(hPipe, singlebuff, 1, &lpBytesRead, &lpTotalBytesAvail, &lpBytesLeftThisMessage);
					if (peakres)
					{
						if (lpBytesRead == 1)
						{
							bool result = ReadFile(hPipe, singlebuff, 1, &lpBytesRead, NULL);
							if (result)
							{
								if (singlebuff[0] != '\0')
								{
									output += singlebuff[0];
								}
							}
							else {
								//debug("timedout");
								singlebuff[0] = '*';
							}
						}
					}
					else {
						//debug("timedout");
						singlebuff[0] = '*';
					}

				}
				if (output != "")
				{
					if (!donereading)
					{
						finaloutput.push_back(output);
						donereading = true;
					}
					// cout << output << endl;
				}
				if (disconnect)
				{
					break;
				}



			}
		}

		t_end = chrono::high_resolution_clock::now();
		elapsed_time_ms = chrono::duration<double, milli>(t_end - t_start).count();

	}

	getpipedatastate = -1;

}

bool restartPipe() {
	if (!disconnect)
	{
		hPipe = CreateNamedPipe(
			lpszPipename,            // pipe name 
			PIPE_ACCESS_INBOUND,     // read/write access  
			PIPE_TYPE_BYTE |      // message-type pipe 
			PIPE_TYPE_BYTE |  // message-read mode 
			PIPE_WAIT,               // blocking mode 
			INSTANCES,               // number of instances 
			BUFSIZE * sizeof(TCHAR),   // output buffer size 
			BUFSIZE * sizeof(TCHAR),   // input buffer size 
			PIPE_TIMEOUT,            // client time-out 
			NULL);                   // default security attributes 

		if (hPipe == INVALID_HANDLE_VALUE)
		{
			//debug("CreateNamedPipe failed with %d. " + GetLastError());

		}
		else {
			// Start a connection for this pipe. 
			//debug("waiting for connection");

			fConnected = ConnectNamedPipe(hPipe, NULL);
			if (!fConnected)
			{
				//debug("ConnectNamedPipe failed with %d. " + GetLastError());

			}
			else {
				getpipeData();
				if (disconnect) {
					DisconnectNamedPipe(hPipe);//Named Pipe Handle
					CloseHandle(hPipe);
				}
			}
		}
	}
	else {
		DisconnectNamedPipe(hPipe);//Named Pipe Handle
		CloseHandle(hPipe);
	}

	return true;
}
bool forcefullyDisconnectPipe() {
	clientPosePipe = CreateFile(
		lpszPipename,   // pipe name  
		GENERIC_WRITE,
		0,              // no sharing 
		NULL,           // default security attributes
		OPEN_EXISTING,  // opens existing pipe 
		0,              // default attributes 
		NULL);          // no template file 

  // Break if the pipe handle is valid. 

	if (clientPosePipe != INVALID_HANDLE_VALUE) {
		this_thread::sleep_for(chrono::milliseconds(2000));
	}
	// Exit if an error other than ERROR_PIPE_BUSY occurs. 

	if (GetLastError() != ERROR_PIPE_BUSY)
	{
		//debug("Could not open pipe");

	}

	CloseHandle(clientPosePipe);

	return true;
}
#pragma endregion

void myThreadRun()
{
	openedview = app->activeViewport();
	while (!openedview) {
		openedview = app->activeViewport();
	}
	AhmsvilleDial_View myView = AhmsvilleDial_View();

	cam = app->activeViewport()->camera();
	cam->isSmoothTransition(false);
	eye = app->activeViewport()->camera()->eye();
	target = app->activeViewport()->camera()->target();
	upvector = app->activeViewport()->camera()->upVector();


	myView.setviewpoints(eye, target, upvector);
	finalpt = myView.rotateviewby(2, 0);


	cam->eye(finalpt);
	cam->upVector(myView.getrotateupvector());

	app->activeViewport()->camera(cam);
	app->activeViewport()->refresh();

	connectthread = async(launch::async, restartPipe);

	while (!stopFlag) {

		if (getpipedatastate == -1)
		{
			//restartPipe();
			if (connectthread.get())
			{
				//debug("restarting pipe");
				DisconnectNamedPipe(hPipe);//Named Pipe Handle
				CloseHandle(hPipe);
				connectthread = async(launch::async, restartPipe);
				getpipedatastate = 0;
			}
		}
		if (donereading)
		{
			//debug(finaloutput);
			vector<string> copyoutput = finaloutput;
			finaloutput.clear();
			for (size_t i = 0; i < copyoutput.size(); i++)
			{
				string currentdatastr = copyoutput[i];
				string substr = ">>";
				string substr2 = "//";
				if (currentdatastr.rfind(substr, 0) == 0) { // pos=0 limits the search to the prefix
					string datastr[4] = { "","","","" };
					float dataflt[4] = { 0,0,0,0 };
					int floatstrpos = 0;
					for (size_t i = 0; i < currentdatastr.length(); i++)
					{
						char c = currentdatastr[i];
						if (c != '\0' && c != '*' && c != '>')
						{
							if (c == '|')
							{
								floatstrpos += 1;
							}
							else
							{
								datastr[floatstrpos] += c;
							}
						}

					}
					for (size_t i = 0; i < size(dataflt); i++)
					{
						//debug(datastr[i]);
						dataflt[i] = atof(datastr[i].c_str());

					}

					if (dataflt[0] != 0 || dataflt[1] != 0 || dataflt[2] != 0 || dataflt[3] != 0)
					{
						eye = app->activeViewport()->camera()->eye();
						target = app->activeViewport()->camera()->target();
						upvector = app->activeViewport()->camera()->upVector();
						myView.setviewpoints(eye, target, upvector);
						float scalef = float(defaultslidesensevalue) * 0.01;
						if (!defaulttransmode)
						{
							myView.translateviewby(dataflt[3] * scalef, dataflt[2] * scalef);
							myView.setviewpoints(eye, myView.gettranslatedtarget(), upvector);
							scalef = float(defaulttiltsensevalue) * 0.01;
							finalpt = myView.rotateviewby(-dataflt[0] * scalef, -dataflt[1] * scalef);

							cam->viewExtents(app->activeViewport()->camera()->viewExtents());
							cam->eye(finalpt);
							cam->upVector(myView.getrotateupvector());
							cam->target(myView.gettranslatedtarget());

							app->activeViewport()->camera(cam);
							app->activeViewport()->refresh();
						}
						else {
							myView.translateviewby(dataflt[3], 0);
							myView.setviewpoints(eye, myView.gettranslatedtarget(), upvector);
							scalef = float(defaulttiltsensevalue) * 0.01;
							finalpt = myView.rotateviewby(-dataflt[0] * scalef, -dataflt[1] * scalef);
							//myView.setviewpoints(finalpt, myView.gettranslatedtarget(), upvector);
							//finalpt = myView.zoomby(dataflt[2] * scalef);
							float newviewextent = (app->activeViewport()->camera()->viewExtents() + (dataflt[2] * scalef)) / app->activeViewport()->camera()->viewExtents();
							cam->viewExtents(app->activeViewport()->camera()->viewExtents() * newviewextent);
							cam->eye(finalpt);
							cam->upVector(myView.getrotateupvector());
							cam->target(myView.gettranslatedtarget());

							app->activeViewport()->camera(cam);
							app->activeViewport()->refresh();
						}


					}

				}
				else if (currentdatastr.rfind(substr2, 0) == 0) { // pos=0 limits the search to the prefix
					//debug(currentdatastr);
					string functionname = "";
					for (size_t i = 0; i < currentdatastr.length(); i++)
					{
						if (currentdatastr[i] != '/' && currentdatastr[i] != '*')
						{
							functionname += currentdatastr[i];
						}
					}
					if (functionname == "FU360_zoomToFit")
					{
						cam = app->activeViewport()->camera();
						cam->isFitView(true);
						cam->isSmoothTransition(false);
						app->activeViewport()->camera(cam);
						app->activeViewport()->refresh();
						cam->isFitView(false);
					}
				}
			}


			donereading = false;
		}

		/*
		eye = app->activeViewport()->camera()->eye();
		target = app->activeViewport()->camera()->target();
		upvector = app->activeViewport()->camera()->upVector();
		myView.setviewpoints(eye, target, upvector);
		finalpt = myView.rotateviewby(2, 0);


		cam->eye(finalpt);
		cam->upVector(myView.getrotateupvector());

		app->activeViewport()->camera(cam);
		app->activeViewport()->refresh();

		*/


		//double randVal = distribution(generator);
		//string additionalInfo = to_string(randVal / 1000.0);
		//app->fireCustomEvent(myCustomEvent, additionalInfo);


		//ui->messageBox(to_string(eye->x()) + "  ||  " + to_string(eye->y()));
		//this_thread::sleep_for(chrono::milliseconds(1000));
	}
	//return "";
}

// InputChange event handler.
class OnInputChangedEventHander : public adsk::core::InputChangedEventHandler
{
public:
	void notify(const Ptr<InputChangedEventArgs>& eventArgs) override
	{

		Ptr<CommandInputs> inputs = eventArgs->inputs();
		if (!inputs)
			return;

		Ptr<CommandInput> cmdInput = eventArgs->input();
		if (!cmdInput)
			return;
		//std::string outd = std::to_string(tiltsense_slider->valueOne()) + "/////" + std::to_string(tiltsense_slider->valueTwo());

		defaulttransmode = transmode->value();
		defaulttiltsensevalue = tiltsense_slider->valueOne();
		defaultslidesensevalue = slidesense_slider->valueOne();
		std::string outd = std::to_string(transmode->value());
		//ui->messageBox(outd);


	}
};

// CommandExecuted event handler.
class OnExecuteEventHander : public adsk::core::CommandEventHandler
{
public:
	void notify(const Ptr<CommandEventArgs>& eventArgs) override
	{

	}
};

// CommandDestroyed event handler
class OnDestroyEventHandler : public adsk::core::CommandEventHandler
{
public:
	void notify(const Ptr<CommandEventArgs>& eventArgs) override
	{
		adsk::terminate();
	}
};

// CommandCreated event handler.
class CommandCreatedEventHandler : public adsk::core::CommandCreatedEventHandler
{
public:
	void notify(const Ptr<CommandCreatedEventArgs>& eventArgs) override
	{
		if (eventArgs)
		{
			// Get the command that was created.
			Ptr<Command> command = eventArgs->command();
			if (command)
			{


				// Connect to the input changed event.
				Ptr<InputChangedEvent> onInputChanged = command->inputChanged();
				if (!onInputChanged)
					return;
				bool isOk = onInputChanged->add(&onInputChangedHandler);
				if (!isOk)
					return;

				// Get the CommandInputs collection associated with the command.
				Ptr<CommandInputs> inputs = command->commandInputs();
				if (!inputs)
					return;


				command->execute();

				// Create bool value input with checkbox style.
				transmode = inputs->addBoolValueInput("trans_zoom", "Translate and Zoom", true, "", false);
				transmode->value(defaulttransmode);

				// Create float slider input with two sliders and visible texts
				tiltsense_slider = inputs->addIntegerSliderCommandInput("tiltsense", "Tilt Sensitivity", 1, 400, false);
				tiltsense_slider->valueOne(defaulttiltsensevalue);
				slidesense_slider = inputs->addIntegerSliderCommandInput("slidsense", "Slide Sensitivity", 1, 400, false);
				slidesense_slider->valueOne(defaultslidesensevalue);


			}
		}
	}
private:
	OnExecuteEventHander onExecuteHandler;
	OnDestroyEventHandler onDestroyHandler;
	OnInputChangedEventHander onInputChangedHandler;
} _cmdCreatedHandler;



extern "C" XI_EXPORT bool run(const char* context)
{

	app = Application::get();
	if (!app)
		return false;

	ui = app->userInterface();

	if (!ui)
		return false;


	// Create the command definition.
	Ptr<CommandDefinitions> commandDefinitions = ui->commandDefinitions();
	if (!commandDefinitions)
		return nullptr;

	// Create a command definition and add a button to the CREATE panel.
	Ptr<CommandDefinition> cmdDef = ui->commandDefinitions()->addButtonDefinition("ahmsvilledialpanel", "Ahmsville Dial Fusion 360", "Ahmsville dial space navigator configuration panel", "");
	if (!cmdDef)
		return false;

	Ptr<ToolbarPanel> createPanel = ui->allToolbarPanels()->itemById("SolidCreatePanel");
	if (!createPanel)
		return false;

	Ptr<CommandControl> ahmsvilledialButton = createPanel->controls()->addCommand(cmdDef);
	if (!ahmsvilledialButton)
		return false;


	// Connect to the command created event.
	Ptr<CommandCreatedEvent> commandCreatedEvent = cmdDef->commandCreated();
	if (!commandCreatedEvent)
		return false;
	commandCreatedEvent->add(&_cmdCreatedHandler);

	customEvent = app->registerCustomEvent(myCustomEvent);
	if (!customEvent)
		return false;
	customEvent->add(&onCustomEvent_);

	stopFlag = false;
	disconnect = false;
	thread myThread(myThreadRun);
	myThread.detach();

	return true;
}

extern "C" XI_EXPORT bool stop(const char* context)
{
	if (ui)
	{
		customEvent->remove(&onCustomEvent_);
		stopFlag = true;
		app->unregisterCustomEvent(myCustomEvent);
		disconnect = true;
		DisconnectNamedPipe(hPipe);
		forcefullyDisconnectPipe();

		Ptr<ToolbarPanel> createPanel = ui->allToolbarPanels()->itemById("SolidCreatePanel");
		if (!createPanel)
			return false;

		Ptr<CommandControl> gearButton = createPanel->controls()->itemById("ahmsvilledialpanel");
		if (gearButton)
			gearButton->deleteMe();

		Ptr<CommandDefinition> cmdDef = ui->commandDefinitions()->itemById("ahmsvilledialpanel");
		if (cmdDef)
			cmdDef->deleteMe();

		//ui->messageBox("Terminating Ahmsville Dial Addin");
		ui = nullptr;
	}

	return true;
}


#ifdef XI_WIN

#include <windows.h>

BOOL APIENTRY DllMain(HMODULE hmodule, DWORD reason, LPVOID reserved)
{
	switch (reason)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

#endif // XI_WIN

void debug(string msgout) {

	AllocConsole();
	freopen("CONOUT$", "w", stdout);
	//printf(msgout);
	cout << msgout << endl;
}