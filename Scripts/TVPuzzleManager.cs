using UnityEngine;

public class TVPuzzleManager : MonoBehaviour
{
    public TVNode tv1;
    public TVNode tv2;
    public TVNode tv3;

   [Header("Passagem TV1 para TV2")]
public CablePassage passage12;

[Header("Passagem TV1 para TV3")]
public CablePassage passage13;
    [Header("Cabo A visual")]

    [Header("Visual do cabo")]
    public LineRenderer cableLine;
    public LineRenderer cableBLine;
    public LineRenderer cableCLine;
    public float cableWidth = 0.08f;

    private bool hasKey = false;
    private bool connected12 = false;
    private bool connected23 = false;
    private bool connectedB = false;
    private TVNode firstSelectedTV;

    void Start()
    {
       if (cableLine != null)
         if (cableBLine != null)
    {
        cableBLine.enabled = false;
    }
{
    cableLine.positionCount = 3;
    cableLine.startWidth = cableWidth;
    cableLine.endWidth = cableWidth;
    cableLine.enabled = false;
    cableLine.useWorldSpace = true;

    cableLine.SetPosition(0, new Vector3(-0.2f, 2.5f, 0f));
    cableLine.SetPosition(1, new Vector3(-0.1f, 2.3f, 0f));
    cableLine.SetPosition(2, new Vector3(0.2f, 1f, 0f));
}
    }

    public void SelectTV(TVNode selectedTV)
    {
        if (firstSelectedTV == null)
        {
            firstSelectedTV = selectedTV;
            Debug.Log("Primeira televisão selecionada: " +
                      selectedTV.name + ". Seleciona a segunda.");
            return;
        }

        if (firstSelectedTV == selectedTV)
        {
            firstSelectedTV = null;
            Debug.Log("Seleção cancelada.");
            return;
        }

        ConnectSelectedTVs(firstSelectedTV, selectedTV);
        firstSelectedTV = null;
    }

    private void ConnectSelectedTVs(TVNode a, TVNode b)
{
    if (a == tv1 && b == tv2)
    {
        ConnectTV1ToTV2();
    }
    else if (a == tv2 && b == tv1)
    {
        ConnectTV2ToTV1();
    }
    else if ((a == tv1 && b == tv3) ||
             (a == tv3 && b == tv1))
    {
        ConnectTV1ToTV3();
    }
    else
    {
        Debug.Log("Esta ligação ainda não está disponível.");
    }
}

    private void ConnectTV1ToTV2()
    {
        if (connected12)
        {
            Debug.Log("O cabo entre TV1 e TV2 já está ligado.");
            return;
        }

        connected12 = true;

        if (passage12 != null)
        {
            passage12.ConnectCable();
        }

        if (cableLine != null)
        {
            cableLine.SetPosition(0, tv1.transform.position);
            cableLine.SetPosition(1, tv2.transform.position);
            cableLine.enabled = true;
        }
        else
        {
            Debug.LogWarning("Falta atribuir o Line Renderer.");
        }

        Debug.Log("Cabo A ligado: TV1 -> TV2. Passagem aberta!");
    }

private void ConnectTV2ToTV1()
{
    connectedB = true;

    if (cableBLine != null)
    {
        cableBLine.enabled = true;
    }
    else
    {
        Debug.LogWarning("O Cabo B não está atribuído no Inspector.");
    }

    Debug.Log("Cabo B ligado: TV2 -> TV1!");
}

    private void ConnectTV1ToTV3()
{
    if (!hasKey)
    {
        Debug.Log("Precisas da chave para ligar o Cabo C!");
        return;
    }

    if (connected23)
    {
        Debug.Log("O Cabo C entre TV1 e TV3 já está ligado.");
        return;
    }

    connected23 = true;
    if (cableCLine != null)
{
    cableCLine.positionCount = 2;
    cableCLine.SetPosition(0, tv1.transform.position);
    cableCLine.SetPosition(1, tv3.transform.position);
    cableCLine.enabled = true;
}
else
{
    Debug.LogWarning("O Cabo C visual não está atribuído no Inspector.");
}

    if (passage13 != null)
    {
        passage13.ConnectCable();
    }
    else
    {
        Debug.LogWarning(
            "Falta atribuir a passagem do Cabo C no Inspector.");
    }

    Debug.Log("Cabo C ligado: TV1 -> TV3. Passagem aberta!");
}

    public void CollectKey()
    {
        hasKey = true;
        Debug.Log("Apanhaste a chave!");
    }
    public bool HasKey()
{
    return hasKey;
}

public bool CableBConnected()
{
    return connectedB;
}
}