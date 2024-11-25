using UnityEngine;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {

        DialogueSystem ds;
        TextArchitect architect;

        public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

        string[] lines = new string[5]
        {
            "1. This is line 5 of the dialogue. It is a VERY LONG Line. HAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA Hola Hello Ohayo.",
            "2. I love cheese. Chloe does not understand fully, but she feels that she can trust this voice. She also feels her instincts screaming danger",
            "3. My name is ChloeThe floating light floats away, and Chloe follows. Unsure of what is to come, what is happening, and who she, Chloe, is to find herself in this situation",
            "4. NOOOOOOOOO....!!!",
            "5. :P"
        };

        private int lineNB;
        void Start()
        {
            lineNB = 0;
            ds = DialogueSystem.Instance;
            architect = new TextArchitect(ds._dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.typewriter;
            architect.speed = 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            if (bm != architect.buildMethod)
            {
                architect.buildMethod = bm;
                architect.Stop();
            }

            if (Input.GetKeyDown(KeyCode.Space) && lineNB < lines.Length)
            {
                if (architect.isBuilding)
                {
                    if (!architect.fasterText)
                        architect.fasterText = true;
                    else 
                        architect.ForceComplete();
                }
                else
                {
                    architect.Build(lines[lineNB]);
                    lineNB++;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A) && lineNB < lines.Length)
            {
                architect.Append(lines[lineNB]);
                lineNB++;
            }
        }
    }
}