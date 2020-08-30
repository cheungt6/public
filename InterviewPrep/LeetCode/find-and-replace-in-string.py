from typing import List

class Solution:
    def findReplaceString3(self, S: str, indexes: List[int], sources: List[str], targets: List[str]) -> str:
        result = ''
        i = 0
        while i < len(S):
            if i in indexes:
                thisI = indexes.index(i)
                source = sources[thisI]
                sourceLen = len(source)
                if S[i:(i+sourceLen)] == source:
                    result = result + targets[thisI]
                    i = i + sourceLen
                else:
                    result += S[i:i+1]
                    i = i + 1
            else:
                result += S[i:i+1]
                i = i + 1
        return result

    def findReplaceString2(self, S: str, indexes: List[int], sources: List[str], targets: List[str]) -> str:
        result = ''
        i = 0
        while i < len(S):
            if i in indexes and S[i:(i+len(sources[indexes.index(i)]))] == sources[indexes.index(i)]:
                result = result + targets[indexes.index(i)]
                i = i + len(sources[indexes.index(i)])
            else:
                result += S[i:i+1]
                i = i + 1
        return result
        
    def findReplaceString1(self, S: str, indexes: List[int], sources: List[str], targets: List[str]) -> str:
        toReplace = {}
        for i in range(len(indexes)):
            if S[indexes[i]:(indexes[i]+len(sources[i]))] == sources[i]:
                toReplace[indexes[i]] = [sources[i], targets[i]]
        result = ''
        i = 0
        while i < len(S):
            if i in toReplace.keys():
                result = result + toReplace[i][1]
                i = i + len(toReplace[i][0])
            else:
                result += S[i:i+1]
                i = i + 1
        return result



sol = Solution()
result = sol.findReplaceString("abcd",[0, 2],["a", "cd"],["eee", "ffff"])
print(result)