# Scenes

## Loading

Scene changes are responsible for most merge conflicts.

Scenes should be kept small, worked on by one person at a time, and loaded additively, i.e.

`SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);`

instead of

`SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);` or `SceneManager.LoadScene("SampleScene");`