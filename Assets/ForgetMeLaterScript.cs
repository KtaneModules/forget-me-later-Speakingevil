using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForgetMeLaterScript : MonoBehaviour
{

    public KMAudio Audio;
    public KMBombInfo bomb;
    public List<KMSelectable> buttons;
    public Renderer[] brends;
    public Renderer bg;
    public Renderer[] framework;
    public Renderer[] leds;
    public Material[] ledon;
    public TextMesh[] digits;

    private static string[] exempt = null;
    private List<int>[] intlist = new List<int>[2] { new List<int> { 0, 0 }, new List<int> { } };
    private List<string> logdigits = new List<string> { };
    private int rule;
    private int buffer;
    private int solvenum = 1;
    private bool firstsolve;
    private bool repeat;
    private bool armed;

    private static int moduleIDCounter;
    private int moduleID;
    private bool moduleSolved;

    private void Awake()
    {
        moduleID = moduleIDCounter++;
        exempt = GetComponent<KMBossModule>().GetIgnoredModules("Forget Me Later", new string[]
        {
            "Forget Me Not",
            "Forget Everything",
            "Forget This",
            "Forget Infinity",
            "Forget Them All",
            "Simon's Stages",
            "Turn The Key",
            "The Time Keeper",
            "Timing is Everything",
            "Alchemy",
            "Cookie Jars",
            "Purgatory",
            "Hogwarts",
            "Souvenir",
            "The Swan",
            "Divided Squares",
            "The Troll",
            "Tallordered Keys",
            "Forget Enigma",
            "Forget Us Not",
            "Organization",
            "Forget Perspective",
            "The Very Annoying Button",
            "The Mildly Annoying Button",
            "Forget Me Later",
            "Simon Supervises",
            "Bad Mouth",
            "Bad TV",
            "Simon Superintends"
        });
        leds[9].material = ledon[0];
        foreach (KMSelectable button in buttons)
        {
            int b = buttons.IndexOf(button);
            button.OnInteract += delegate () { ButtonPress(b); return false; };
        }
    }

    void Start()
    {
        if (bomb.GetSolvableModuleNames().Where(x => !exempt.Contains(x)).Count() == 0)
        {
            StartCoroutine(SolveAnim());
        }
        else
        {
            for (int i = 0; i < bomb.GetSolvableModuleNames().Where(x => !exempt.Contains(x)).Count(); i++)
            {
                intlist[0].Add(Random.Range(0, 10));
                logdigits.Add(intlist[0][i + 2].ToString());
                Debug.Log(logdigits[i]);
            }
            Debug.LogFormat("[Forget Me Later #{0}]The initial digits were {1}", moduleID, string.Join(string.Empty, logdigits.ToArray()));
            intlist[1].Add(Random.Range(0, 10));
            intlist[1].Add(Random.Range(0, 10));
            StartCoroutine(Sequence());
        }
    }

    private void ButtonPress(int b)
    {
        if (moduleSolved == false && armed == true)
        {
            Debug.LogFormat("[Forget Me Later #{0}]Button {1} pressed", moduleID, (b + 1) % 10);
            if (bomb.GetSolvedModuleNames().Where(x => !exempt.Contains(x)).Count() != bomb.GetSolvableModuleNames().Where(x => !exempt.Contains(x)).Count())
            {
                armed = false;
                buttons[b].AddInteractionPunch(1f);
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                digits[10].text = "X" + intlist[1][solvenum - 2] + intlist[1][solvenum - 1] + intlist[1][solvenum] + "X";
                if ((b + 1) % 10 != intlist[1][solvenum])
                {
                    GetComponent<KMBombModule>().HandleStrike();
                    repeat = true;
                }
            }
            else
            {
                buttons[b].AddInteractionPunch(1f);
                Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
                if ((b + 1) % 10 != intlist[1][solvenum])
                {
                    GetComponent<KMBombModule>().HandleStrike();
                }
                else
                {
                    StartCoroutine(SolveAnim());
                }
            }
        }
    }

    void Update()
    {
        if (moduleSolved == false)
        {
            buffer++;
            if (buffer == 9)
            {
                buffer = 0;
                if (bomb.GetSolvedModuleNames().Where(x => !exempt.Contains(x)).Count() + 1 != solvenum)
                {
                    solvenum = bomb.GetSolvedModuleNames().Where(x => !exempt.Contains(x)).Count() + 1;
                    if (firstsolve == false)
                    {
                        firstsolve = true;
                        StopAllCoroutines();
                        armed = true;
                        StartCoroutine(Transformation());
                    }
                    else
                    {
                        if (armed == true)
                        {
                            GetComponent<KMBombModule>().HandleStrike();
                            Debug.LogFormat("[Forget Me Later #{0}]Error: Input not received", moduleID);
                        }
                        if (repeat == true)
                        {
                            repeat = false;
                            StartCoroutine(Sequence());
                        }
                        else
                        {
                            NextStage();
                        }
                    }
                }
            }
        }
    }

    void NextStage()
    {
        armed = true;
        foreach (Renderer led in leds)
        {
            led.material = ledon[0];
        }
        leds[(solvenum + 8) % 10].material = ledon[1];
        digits[10].text = "X" + intlist[1][solvenum - 2] + intlist[1][solvenum - 1] + "-X";
        rule = Random.Range(0, 60);
        digits[11].text = rule.ToString();
        Debug.LogFormat("[Forget Me Later #{0}]At stage {1}, the rule that applied was {2}", moduleID, solvenum - 1, rule);
        switch (rule)
        {
            case 0:
                intlist[1].Add(intlist[0][solvenum]);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}", moduleID, intlist[0][solvenum]);
                break;
            case 1:
                intlist[1].Add((intlist[0][solvenum] + 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}: {1} + 1 = {2}", moduleID, intlist[0][solvenum], intlist[0][solvenum] + 1);
                break;
            case 2:
                intlist[1].Add((2 * intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}: 2 * {1} = {2}", moduleID, intlist[0][solvenum], 2 * intlist[0][solvenum]);
                break;
            case 3:
                intlist[1].Add((intlist[1][solvenum - 1] + intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: {1} + {2} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], intlist[0][solvenum] + intlist[1][solvenum - 1]);
                break;
            case 4:
                intlist[1].Add(Mathf.Abs(intlist[1][solvenum - 1] - intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: {2} - {1} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], intlist[1][solvenum - 1] - intlist[0][solvenum]);
                break;
            case 5:
                intlist[1].Add(Mathf.Abs(intlist[1][solvenum - 1] - intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: {2} - {1} = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], intlist[1][solvenum - 1] - intlist[1][solvenum - 2]);
                break;
            case 6:
                intlist[1].Add(Mathf.Abs(intlist[1][solvenum - 2] - intlist[0][solvenum]));
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: {2} - {1} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], intlist[1][solvenum - 2] - intlist[0][solvenum]);
                break;
            case 7:
                intlist[1].Add((intlist[0][solvenum] + intlist[1][solvenum - 1] + 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: {1} + {2} + 1 = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], intlist[0][solvenum] + intlist[1][solvenum - 1] + 1);
                break;
            case 8:
                intlist[1].Add((intlist[0][solvenum] + intlist[1][solvenum - 2] + 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: {1} + {2} + 1 = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], intlist[0][solvenum] + intlist[1][solvenum - 2] + 1);
                break;
            case 9:
                intlist[1].Add((intlist[1][solvenum - 1] + intlist[1][solvenum - 2] + 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: {1} + {2} + 1 = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], intlist[1][solvenum - 1] + intlist[1][solvenum - 2] + 1);
                break;
            case 10:
                intlist[1].Add(intlist[1][solvenum - 1]);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}", moduleID, intlist[1][solvenum - 1]);
                break;
            case 11:
                intlist[1].Add((intlist[1][solvenum - 1] + 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}: {1} + 1 = {2}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 1] + 1);
                break;
            case 12:
                intlist[1].Add((2 * intlist[1][solvenum - 1]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}: 2 * {1} = {2}", moduleID, intlist[1][solvenum - 1], 2 * intlist[1][solvenum - 1]);
                break;
            case 13:
                intlist[1].Add((intlist[1][solvenum - 1] + intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: {1} + {2} = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], intlist[1][solvenum - 1] + intlist[1][solvenum - 2]);
                break;
            case 14:
                intlist[1].Add(Mathf.Abs(intlist[0][solvenum] - 1));
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}: {1} - 1", moduleID, intlist[0][solvenum], intlist[0][solvenum] - 1);
                break;
            case 15:
                intlist[1].Add(Mathf.Abs(intlist[1][solvenum - 1] - 1));
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}: {1} - 1", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 1] - 1);
                break;
            case 16:
                intlist[1].Add(Mathf.Abs(intlist[1][solvenum - 2] - 1));
                Debug.LogFormat("[Forget Me Later #{0}]Second-last input was {1}: {1} - 1", moduleID, intlist[1][solvenum - 2], intlist[1][solvenum - 2] - 1);
                break;
            case 17:
                intlist[1].Add(Mathf.Abs(intlist[0][solvenum] + intlist[1][solvenum - 1] - 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: {1} + {2} - 1 = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], intlist[0][solvenum] + intlist[1][solvenum - 1] - 1);
                break;
            case 18:
                intlist[1].Add(Mathf.Abs(intlist[0][solvenum] + intlist[1][solvenum - 2] - 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: {1} + {2} - 1 = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], intlist[0][solvenum] + intlist[1][solvenum - 2] - 1);
                break;
            case 19:
                intlist[1].Add(Mathf.Abs(intlist[1][solvenum - 2] + intlist[1][solvenum - 1] - 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: {1} + {2} - 1 = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], intlist[1][solvenum - 1] + intlist[1][solvenum - 2] - 1);
                break;
            case 20:
                intlist[1].Add(intlist[1][solvenum - 2]);
                Debug.LogFormat("[Forget Me Later #{0}]Second-last input was {1}", moduleID, intlist[1][solvenum - 2]);
                break;
            case 21:
                intlist[1].Add((intlist[1][solvenum - 2] + 1) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Second-last input was {1}: {1} + 1 = {2}", moduleID, intlist[1][solvenum - 2], intlist[1][solvenum - 2] + 1);
                break;
            case 22:
                intlist[1].Add((2 * intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Second-last input was {1}: 2 * {1} = {2}", moduleID, intlist[1][solvenum - 2], 2 * intlist[1][solvenum - 2]);
                break;
            case 23:
                intlist[1].Add((intlist[1][solvenum - 2] + intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: {1} + {2} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], intlist[0][solvenum] + intlist[1][solvenum - 2]);
                break;
            case 24:
                intlist[1].Add(2 * (intlist[1][solvenum - 1] + intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 2 * ({1} + {2}) = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], 2 * (intlist[1][solvenum - 1] + intlist[1][solvenum - 2]));
                break;
            case 25:
                intlist[1].Add(2 * (intlist[0][solvenum] + intlist[1][solvenum - 1]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: 2 * ({1} + {2}) = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 2 * (intlist[0][solvenum] + intlist[1][solvenum - 1]));
                break;
            case 26:
                intlist[1].Add((2 * (intlist[0][solvenum] + intlist[1][solvenum - 2])) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: 2 * ({1} + {2}) = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], 2 * (intlist[0][solvenum] + intlist[1][solvenum - 2]));
                break;
            case 27:
                intlist[1].Add((2 * Mathf.Abs(intlist[1][solvenum - 1] - intlist[1][solvenum - 2])) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 2 * ({1} - {2}) = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], 2 * (intlist[1][solvenum - 1] - intlist[1][solvenum - 2]));
                break;
            case 28:
                intlist[1].Add((2 * Mathf.Abs(intlist[1][solvenum - 1] - intlist[0][solvenum])) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: 2 * ({1} - {2}) = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 2 * (intlist[0][solvenum] - intlist[1][solvenum - 1]));
                break;
            case 29:
                intlist[1].Add((2 * Mathf.Abs(intlist[1][solvenum - 2] - intlist[0][solvenum])) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: 2 * ({1} - {2}) = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], 2 * (intlist[0][solvenum] - intlist[1][solvenum - 2]));
                break;
            case 30:
                intlist[1].Add((3 * intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}: 3 * {1} = {2}", moduleID, intlist[0][solvenum], 3 * intlist[0][solvenum]);
                break;
            case 31:
                intlist[1].Add((3 * intlist[1][solvenum - 1]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}: 3 * {1} = {2}", moduleID, intlist[1][solvenum - 1], 2 * intlist[1][solvenum - 1]);
                break;
            case 32:
                intlist[1].Add((3 * intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Second-last input was {2}: 3 * {1} = {2}", moduleID, intlist[2][solvenum - 2], 3 * intlist[1][solvenum - 2]);
                break;
            case 33:
                intlist[1].Add((3 * (intlist[1][solvenum - 1] + intlist[1][solvenum - 2])) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 3 * ({1} + {2}) = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], 3 * (intlist[1][solvenum - 1] + intlist[1][solvenum - 2]));
                break;
            case 34:
                intlist[1].Add((3 * intlist[1][solvenum - 1] + intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 3 * {1} + {2} = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], 3 * intlist[1][solvenum - 1] + intlist[1][solvenum - 2]);
                break;
            case 35:
                intlist[1].Add((3 * intlist[0][solvenum] + intlist[1][solvenum - 1]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: 3 * {1} + {2} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 3 * intlist[0][solvenum] + intlist[1][solvenum - 1]);
                break;
            case 36:
                intlist[1].Add((3 * intlist[0][solvenum] + intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: 3 * {1} + {2} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], 3 * intlist[0][solvenum] + intlist[1][solvenum - 2]);
                break;
            case 37:
                intlist[1].Add((3 * intlist[1][solvenum - 1] + intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: 3 * {2} + {1} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 3 * intlist[1][solvenum - 1] + intlist[0][solvenum]);
                break;
            case 38:
                intlist[1].Add((3 * intlist[1][solvenum - 2] + intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: 3 * {2} + {1} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 3 * intlist[1][solvenum - 2] + intlist[0][solvenum]);
                break;
            case 39:
                intlist[1].Add((3 * intlist[1][solvenum - 2] + intlist[1][solvenum - 1]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 3 * {2} + {1} = {3}", moduleID, intlist[1][solvenum - 1], intlist[2][solvenum - 2], 3 * intlist[1][solvenum - 2] + intlist[1][solvenum - 1]);
                break;
            case 40:
                intlist[1].Add((intlist[0][solvenum] + 5) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}: {1} + 5 = {2}", moduleID, intlist[0][solvenum], intlist[0][solvenum] + 5);
                break;
            case 41:
                intlist[1].Add((intlist[1][solvenum - 1] + 5) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}: {1} + 5 = {2}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 1] + 5);
                break;
            case 42:
                intlist[1].Add((intlist[1][solvenum - 2] + 5) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Second-last input was {1}: {1} + 5 = {2}", moduleID, intlist[1][solvenum - 2], intlist[1][solvenum - 2] + 5);
                break;
            case 43:
                intlist[1].Add((2 * intlist[1][solvenum - 1] + intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2},: 2 * {2} + {1} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 2 * intlist[1][solvenum - 1] + intlist[0][solvenum]);
                break;
            case 44:
                intlist[1].Add((2 * intlist[1][solvenum - 2] + intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: 2 * {2} + {1} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], 2 * intlist[1][solvenum - 2] + intlist[0][solvenum]);
                break;
            case 45:
                intlist[1].Add((2 * intlist[0][solvenum] + intlist[1][solvenum - 1]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: 2 * {1} + {2} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 2 * intlist[0][solvenum] + intlist[1][solvenum - 1]);
                break;
            case 46:
                intlist[1].Add((2 * intlist[0][solvenum] + intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: 2 * {1} + {2} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], 2 * intlist[0][solvenum] + intlist[1][solvenum - 2]);
                break;
            case 47:
                intlist[1].Add(Mathf.Abs((2 * intlist[1][solvenum - 1] - intlist[1][solvenum - 2])) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 2 * {1} - {2} = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], 2 * intlist[1][solvenum - 1] - intlist[1][solvenum - 2]);
                break;
            case 48:
                intlist[1].Add(Mathf.Abs((2 * intlist[1][solvenum - 1] - intlist[0][solvenum])) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: 2 * {2} - {1} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 2 * intlist[1][solvenum - 1] - intlist[0][solvenum]);
                break;
            case 49:
                intlist[1].Add(Mathf.Abs((2 * intlist[1][solvenum - 2] - intlist[1][solvenum - 1])) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 2 * {2} - {1} = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], 2 * intlist[1][solvenum - 2] - intlist[1][solvenum - 1]);
                break;
            case 50:
                intlist[1].Add(9 - intlist[0][solvenum]);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}: 9 - {1} = {2}", moduleID, intlist[0][solvenum], 9 - intlist[0][solvenum]);
                break;
            case 51:
                intlist[1].Add(9 - intlist[1][solvenum - 1]);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}: 9 - {1} = {2}", moduleID, intlist[1][solvenum - 1], 9 - intlist[1][solvenum - 1]);
                break;
            case 52:
                intlist[1].Add(9 - intlist[1][solvenum - 2]);
                Debug.LogFormat("[Forget Me Later #{0}]Second-last input was {1}: 9 - {1} = {2}", moduleID, intlist[1][solvenum - 2], 9 - intlist[1][solvenum - 2]);
                break;
            case 53:
                intlist[1].Add((18 - intlist[0][solvenum] - intlist[1][solvenum - 1]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: 18 - {1} - {2} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 18 - intlist[0][solvenum] - intlist[1][solvenum - 1]);
                break;
            case 54:
                intlist[1].Add((18 - intlist[0][solvenum] - intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: 18 - {1} - {2} = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], 18 - intlist[0][solvenum] - intlist[1][solvenum - 2]);
                break;
            case 55:
                intlist[1].Add((18 - intlist[1][solvenum - 1] - intlist[1][solvenum - 2]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 18 - {1} - {2} = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], 18 - intlist[1][solvenum - 2] - intlist[1][solvenum - 2]);
                break;
            case 56:
                intlist[1].Add((18 - 2 * intlist[0][solvenum]) % 10);
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}: 18 - 2 * {1} = {2}", moduleID, intlist[0][solvenum], 18 - 2 * intlist[0][solvenum]);
                break;
            case 57:
                intlist[1].Add(9 - Mathf.Abs(intlist[1][solvenum - 1] - intlist[0][solvenum]));
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Last input was {2}: 9 - |{1} - {2}| = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 1], 9 - Mathf.Abs(intlist[0][solvenum] - intlist[1][solvenum - 1]));
                break;
            case 58:
                intlist[1].Add(9 - Mathf.Abs(intlist[1][solvenum - 1] - intlist[1][solvenum - 2]));
                Debug.LogFormat("[Forget Me Later #{0}]Last input was {1}, Second-last input was {2}: 9 - |{1} - {2}| = {3}", moduleID, intlist[1][solvenum - 1], intlist[1][solvenum - 2], 9 - Mathf.Abs(intlist[1][solvenum - 1] - intlist[1][solvenum - 2]));
                break;
            case 59:
                intlist[1].Add(9 - Mathf.Abs(intlist[0][solvenum] - intlist[1][solvenum - 2]));
                Debug.LogFormat("[Forget Me Later #{0}]Received digit was {1}, Second-last input was {2}: 9 - |{1} - {2}| = {3}", moduleID, intlist[0][solvenum], intlist[1][solvenum - 2], 9 - Mathf.Abs(intlist[0][solvenum] - intlist[1][solvenum - 2]));
                break;
        }
        Debug.LogFormat("[Forget Me Later #{0}]The correct button to press was {1}", moduleID, intlist[1][solvenum]);
    }

    private IEnumerator Sequence()
    {
        foreach (Renderer led in leds)
        {
            led.material = ledon[0];
        }
        if(firstsolve == true)
        {
            digits[10].text = "-----";
            digits[11].text = "--";
            yield return new WaitForSeconds(3);
        }
        int[] lim = new int[2] { bomb.GetSolvedModuleNames().Where(x => !exempt.Contains(x)).Count(), bomb.GetSolvableModuleNames().Where(x => !exempt.Contains(x)).Count() };
        float[] waitTime = new float[9] { 0.75f, 0.6875f, 0.625f, 0.5625f, 0.5f, 0.4375f, 0.375f, 0.3125f, 0.25f };
        for (int i = lim[0]; i < lim[1]; i++)
        {
            digits[10].text = "--" + intlist[0][i + 2].ToString() + "--";
            digits[11].text = (i + 1).ToString();
            leds[i % 10].material = ledon[1];
            leds[(i + 9) % 10].material = ledon[0];
            yield return new WaitForSeconds(waitTime[Mathf.Min(Mathf.FloorToInt(i / 10), 8)]);
            if (i == lim[1] - 1)
            {
                if (firstsolve == false)
                {
                    i = -1;
                    foreach (Renderer led in leds)
                    {
                        led.material = ledon[0];
                    }
                }
                else
                {
                    NextStage();
                }
            }
        }
    }

    private IEnumerator Transformation()
    {
        Audio.PlaySoundAtTransform("Screech", transform);
        digits[10].text = "-----";
        digits[11].text = "--";
        foreach (Renderer led in leds)
        {
            led.material = ledon[0];
        }
        for (int i = 0; i < 51; i++)
        {
            bg.material.color -= new Color32(4, 4, 4, 0);
            foreach (Renderer brend in brends)
            {
                brend.material.color -= new Color32(5, 5, 5, 0);
            }
            foreach (Renderer frame in framework)
            {
                frame.material.color += new Color32(5, 5, 5, 0);
            }
            for (int j = 0; j < 12; j++)
            {
                if (j < 10)
                {
                    digits[j].color += new Color32(5, 0, 0, 0);
                }
                else
                {
                    digits[j].color -= new Color32(0, 5, 5, 0);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        NextStage();
    }

    private IEnumerator SolveAnim()
    {
        digits[10].text = string.Empty;
        digits[11].text = string.Empty;
        foreach (Renderer led in leds)
        {
            led.material = ledon[0];
        }
        for (int i = 0; i < 10; i++)
        {
            leds[i].material = ledon[1];
            yield return new WaitForSeconds(0.2f);
            if (i == 9)
            {
                GetComponent<KMBombModule>().HandlePass();
                moduleSolved = true;
            }
        }
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} 0-9 [presses button labelled with this digit]";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        if ("1234567890".Contains(command))
        {
            if(armed == true)
            {
                yield return null;
                buttons["1234567890".IndexOf(command)].OnInteract();
            }
            else
            {
                yield return "sendtochaterror Forget Me Later cannot be interacted with yet";
            }
        }
    }
}