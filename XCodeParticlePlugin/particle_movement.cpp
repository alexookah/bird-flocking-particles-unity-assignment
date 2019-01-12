#define EXPORT_API
#include <cmath>
#include <iostream>

extern "C"
{
/*    const EXPORT_API void centralGravity(float* particle_states,float* center,float dt)
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
    }*/
    
    const EXPORT_API void physicsStep(float* particle_states, float* gravity, float* bounds, float damping_percentage, int attractors_count, float* attractor_data, float dt, bool bounds_is_circle)
    {
        float* x=&particle_states[0];
        float* v=&particle_states[3];
        float a[3]={0, 0, 0};
        
        //attractos will have:
        
        //float gravity[3] = {0, -1.0f, 0};
        //float bounds[6] = {-10.0f, 0.0f, -10.0f, 10.0f, 20.0f, 10.0f};
        
        
        
        for (int i = 0; i < attractors_count; i++) {
            float distance_squared = 0.0f;
            float distance[3];
            for(int d=0;d<3;d++){
                distance[d] = x[d] - attractor_data[4 * i + d];
                distance_squared +=  distance[d] * distance[d];
            }
            //distance_cubed *= std::sqrt(distance_cubed);
            for(int d=0;d<3;d++){
                a[d] += attractor_data[4 * i + 3] * distance[d] / distance_squared;
            }
        }
        
        for(int d=0;d<3;d++){
            a[d] += gravity[d];
        }
        
        if (bounds_is_circle) {
            
            float* bounds_circle = &bounds[0];
            
            float circle_x = bounds_circle[0];
            float circle_y = bounds_circle[1];
            float circle_z = bounds_circle[2];
            float circle_r = bounds_circle[3];
            
            float dist_X = x[0] - circle_x;
            float dist_Y = x[1] - circle_y;
            float dist_Z = x[2] - circle_z;
            
            float r_2_before = dist_X * dist_X + dist_Y * dist_Y + dist_Z * dist_Z;
            
            for(int d=0;d<3;d++){
                v[d] += a[d] * dt;
                x[d] += v[d] * dt;
            }
            
            dist_X = x[0] - circle_x;
            dist_Y = x[1] - circle_y;
            dist_Z = x[2] - circle_z;
            
            float r_2_after = dist_X * dist_X + dist_Y * dist_Y + dist_Z * dist_Z;
            
            if (r_2_after > circle_r * circle_r) {
                float r_before = std::sqrt(r_2_before);
                float r_after = std::sqrt(r_2_after);
                float r_difference = r_after - r_before;
                if (r_difference < 0.01f) {
                    r_difference = 0.01f;
                }
                float rewind_t = dt * (r_after - circle_r) / r_difference;
                
                // peform the rewind
                for(int d=0;d<3;d++){
                    v[d] -= a[d] * rewind_t;
                    x[d] -= v[d] * rewind_t;
                }
                
                dist_X = x[0] - circle_x;
                dist_Y = x[1] - circle_y;
                dist_Z = x[2] - circle_z;
                
                float force_magnitude = (2 - damping_percentage) * (v[0] * dist_X + v[1] * dist_Y + v[2] * dist_Z) / (circle_r * circle_r);
                
                v[0] -= force_magnitude * dist_X;
                v[1] -= force_magnitude * dist_Y;
                v[2] -= force_magnitude * dist_Z;
                
                // go forward in time (the time that was rewinded)
                for(int d=0;d<3;d++){
                    v[d] += a[d] * rewind_t;
                    x[d] += v[d] * rewind_t;
                }
            }

        } else {
            
            for(int d=0;d<3;d++){
                v[d] += a[d] * dt;
                x[d] += v[d] * dt;
            }
            
            float* bounds_low = &bounds[0];
            float* bounds_high = &bounds[3];
            
            for (int d = 0; d < 3; d++) {
                if (x[d] < bounds_low[d]) {
                    v[d] = -v[d] * (1.0f - damping_percentage);
                    x[d] += 2 * (bounds_low[d] - x[d]);
                }
                if (x[d] > bounds_high[d]) {
                    v[d] = -v[d] * (1.0f - damping_percentage);
                    x[d] += 2 * (bounds_high[d] - x[d]);
                }
            }
        }
    }
    
} // end of export C block
