#define EXPORT_API __declspec(dllexport) // Visual Studio needs to annotate exported functions with this
#include <cmath>

extern "C"
{
    const EXPORT_API void centralGravity(float* particle_states,float* center,float dt)
    {
        float* x=&particle_states[0];
        float* v=&particle_states[3];
        
        float gravity[3]; //"gravity" here is just a direction from the particle's position to the specified center
        for(int d=0;d<3;d++){
            gravity[d]=center[d]-x[d];
        }

        float magnitude=std::sqrt(gravity[0]*gravity[0] + gravity[1]*gravity[1] + gravity[2]*gravity[2]);
        float epsilon=1e-10f;

        for(int d=0;d<3;d++){
            gravity[d]/=(magnitude+epsilon);
        }
        
        for(int d=0;d<3;d++){
            v[d]+=gravity[d]*dt;
        }
        for(int d=0;d<3;d++){
            x[d]+=v[d]*dt;
        }
    }
    
} // end of export C block
