using System.Linq;
using UnityEngine;

public enum TypeBuild { Outside, Lighthouse, Mine, Farm, Woodcutter}

public enum ActionType { Select, Disable, Agree }

[System.Serializable]
public class CellBuild
{
    public Transform cell;
    public TypeBuild type;
}

[System.Serializable]
public class BuildInside
{
    public TypeBuild type;
    public GameObject inside;
}

public class BuildManager : MonoBehaviour
{
    [SerializeField] private CellBuild[] _cellsBuilds;
    [SerializeField] private BuildInside[] _buildsInsides;

    private TypeBuild _currentBuild = TypeBuild.Outside;

    private void Start()
    {
        CameraManager.Instance.OnBuildEnter += OnBuildEnter;
        CameraManager.Instance.OnBuildExit += OnBuildExit;
    }

    private void OnBuildEnter(TypeBuild type)
    {
        BuildInside build = _buildsInsides.FirstOrDefault(inside => inside.type == type);
        build.inside.SetActive(true);
        _currentBuild = build.type;
    }

    private void OnBuildExit(TypeBuild type)
    {
        BuildInside exitBuild = _buildsInsides.FirstOrDefault(inside => inside.type == type);
        exitBuild.inside.SetActive(false);
        _currentBuild = TypeBuild.Outside;
    }
}
