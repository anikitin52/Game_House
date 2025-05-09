#version 460 core

out vec4 FragColor;
in vec2 texCoord;

uniform sampler2D texture0;

void main() 
{
    vec4 texColor = texture(texture0, texCoord);
    
    float threshold = 0.05;
    if (texColor.r < threshold && texColor.g < threshold && texColor.b < threshold) {
        texColor.a = 0.4;
    }
    
    FragColor = texColor;
}